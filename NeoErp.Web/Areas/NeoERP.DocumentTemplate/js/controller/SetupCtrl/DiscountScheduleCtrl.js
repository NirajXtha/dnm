(function(){
    'use strict';

    DTModule.controller('DiscountScheduleCtrl', function($scope, $http, $timeout){
        // Model
        $scope.discountSchedule = {
            effectiveDate: new Date(),
            currencyCode: '',
            exchangeRate: 1,
            areaCode: '',
            documentCode: '',
            chargeCode: '',
            partyType: 'customer',
            partyName: '',
            partyCode: '',
            optionType: 'percentage'
        };

        $scope.currencyList = [];
        $scope.chargeCodeList = [];
        $scope.documentList = [];

        $scope.selectedParties = []; // array of { CODE, NAME }
        $scope.selectedItems = [];   // array of { ITEM_CODE, ITEM_EDESC, MU_CODE }
        $scope.itemsSummary = '';
        // Persist group header input values (by parent -> field -> value) to survive grid rebinds
        $scope._groupHeaderValues = {};

        // Cache full trees with individuals per party type (to mirror RateSchedule)
        $scope._fullTreeByParty = {
            customer: null,
            dealer: null,
            supplier: null
        };

        function getSelectedTreeElementId() {
            if ($scope.discountSchedule.partyType === 'customer') return '#selectedCustomersTree';
            if ($scope.discountSchedule.partyType === 'dealer') return '#selectedDealersTree';
            if ($scope.discountSchedule.partyType === 'supplier') return '#selectedSuppliersTree';
            return '#selectedCustomersTree';
        }

        function buildSelectedTreeData() {
            var list = ($scope.selectedParties || []).map(function(p){
                return { id: p.CODE, text: p.NAME || p.CODE, hasChildren: false };
            });
            return list;
        }

        function getSourcePartyTreeId() {
            if ($scope.discountSchedule.partyType === 'customer') return '#customerTreeView';
            if ($scope.discountSchedule.partyType === 'dealer') return '#dealerTreeView';
            if ($scope.discountSchedule.partyType === 'supplier') return '#supplierTreeView';
            return '#customerTreeView';
        }

        // Build hierarchy from flat groups
        function buildHierarchy(rows, idField, parentField, textField) {
            var byId = {}, roots = [];
            (rows || []).forEach(function(r){
                var id = r[idField];
                var pid = r[parentField];
                var text = r[textField] || r.EDESC || r.Edesc || r.NAME || id;
                var node = byId[id] || { items: [] };
                node.id = id;
                node.text = text;
                node.hasChildren = true;
                byId[id] = node;
                node._pid = pid;
            });
            Object.keys(byId).forEach(function(id){
                var n = byId[id];
                var pid = n._pid;
                if (pid && byId[pid] && pid !== id) byId[pid].items.push(n); else roots.push(n);
                delete n._pid;
            });
            return roots;
        }

        // Populate individuals under groups similar to RateSchedule.populateAllIndividualsForTree
        function ensureFullTreeWithIndividuals(partyType) {
            if ($scope._fullTreeByParty[partyType]) return Promise.resolve($scope._fullTreeByParty[partyType]);
            var groupsUrl, idF, pF, tF, cfg, individualsApiUrl;
            if (partyType === 'customer') { groupsUrl = '/api/SetupApi/GetCustomerGroups'; idF='MASTER_CUSTOMER_CODE'; pF='PRE_CUSTOMER_CODE'; tF='CUSTOMER_EDESC'; cfg={ codeField:'CUSTOMER_CODE', nameField:'CUSTOMER_EDESC', masterField:'MASTER_CUSTOMER_CODE' }; individualsApiUrl='/api/SetupApi/GetChildOfCustomerByGroup'; }
            else if (partyType === 'dealer') { groupsUrl = '/api/SetupApi/GetDealerGroups'; idF='MASTER_PARTY_CODE'; pF='PRE_PARTY_CODE'; tF='PARTY_TYPE_EDESC'; cfg={ codeField:'PARTY_TYPE_CODE', nameField:'PARTY_TYPE_EDESC', masterField:'MASTER_PARTY_CODE' }; individualsApiUrl='/api/SetupApi/GetChildOfDealerByGroup'; }
            else if (partyType === 'supplier') { groupsUrl = '/api/SetupApi/GetSupplierGroups'; idF='MASTER_SUPPLIER_CODE'; pF='PRE_SUPPLIER_CODE'; tF='SUPPLIER_EDESC'; cfg={ codeField:'SUPPLIER_CODE', nameField:'SUPPLIER_EDESC', masterField:'MASTER_SUPPLIER_CODE' }; individualsApiUrl='/api/SetupApi/GetChildOfsupplierByGroup'; }
            else { return Promise.resolve([]); }

            return $http.get(groupsUrl).then(function(resp){
                var rows = Array.isArray(resp.data) ? resp.data : (resp.data && (resp.data.DATA || resp.data.Data)) || [];
                var tree = buildHierarchy(rows, idF, pF, tF);
                // Index by id (string keys to avoid type mismatch)
                var idx = {};
                (function index(nodes){
                    (nodes||[]).forEach(function(n){
                        idx[String(n.id)] = n;
                        if (n.items && n.items.length) index(n.items);
                    });
                })(tree);
                var groupIds = Object.keys(idx);
                // Fetch individuals per group (same as selector) and attach
                var requests = groupIds.map(function(gid){
                    var url = individualsApiUrl + '?groupCode=' + encodeURIComponent(gid);
                    return $http.get(url).then(function(resp){
                        var raw = Array.isArray(resp.data) ? resp.data : (resp.data && (resp.data.DATA || resp.data.Data)) || [];
                        var leaves = (raw || []).map(function(x){
                            var code = x[cfg.codeField] || x.CUSTOMER_CODE || x.PARTY_TYPE_CODE || x.SUPPLIER_CODE || x.CHILD_CUSTOMER_CODE || x.CHILD_PARTY_CODE || x.CHILD_SUPPLIER_CODE || x.CODE || x.ID;
                            var name = x[cfg.nameField] || x.CUSTOMER_EDESC || x.PARTY_TYPE_EDESC || x.SUPPLIER_EDESC || x.CHILD_CUSTOMER_EDESC || x.CHILD_PARTY_EDESC || x.CHILD_SUPPLIER_EDESC || x.NAME || x.EDESC || x.TEXT;
                            return { code: code, name: name };
                        }).filter(function(it){ return it.code; });
                        var parent = idx[String(gid)];
                        if (parent) {
                            parent.items = parent.items || [];
                            leaves.forEach(function(it){
                                var exists = parent.items.some(function(c){ return c && String(c.id) === String(it.code); });
                                if (!exists) parent.items.push({ id: String(it.code), text: it.name, hasChildren: false });
                            });
                            parent.hasChildren = Array.isArray(parent.items) && parent.items.length>0;
                        }
                    });
                });
                return Promise.all(requests).then(function(){
                    $scope._fullTreeByParty[partyType] = tree;
                    return tree;
                });
            });
        }

        // Use in-memory tree data (with individuals) to find paths
        function findPathsInData(nodes, code) {
            var out = [];
            function dfs(arr, trail){
                for (var i=0;i<(arr||[]).length;i++){
                    var n = arr[i];
                    var trail2 = trail.concat([n]);
                    var isMatch = (String(n.id) === String(code)) || (n.originalCode != null && String(n.originalCode) === String(code));
                    if (isMatch) out.push(trail2);
                    if (n.items && n.items.length) dfs(n.items, trail2);
                }
            }
            dfs(nodes, []);
            return out;
        }

        function buildSpecificPathTreeFromData(treeData, chosen, checkedMasters) {
            var pathMap = {};
            checkedMasters = Array.isArray(checkedMasters) ? checkedMasters.map(String) : [];
            function ensureGroupNode(id, text){
                if (!pathMap[id]) pathMap[id] = { id:id, text:text, items:[], hasChildren:false, _leafSet:{} };
                if (!pathMap[id]._leafSet) pathMap[id]._leafSet = {};
                return pathMap[id];
            }
            function addLeafToGroup(groupNode, code, name){
                if (!groupNode._leafSet[code]){ groupNode._leafSet[code]=true; (groupNode.items=groupNode.items||[]).push({id:code,text:name,hasChildren:false}); groupNode.hasChildren=true; }
            }
            // Build index of group nodes in provided treeData
            var groupIndex = {};
            (function index(nodes){ (nodes||[]).forEach(function(n){ groupIndex[String(n.id)] = n; if (n.items && n.items.length) index(n.items); }); })(treeData);

            // Helper: deep-clone a subtree (groups + leaves) into pathMap structure
            function cloneSubtree(node) {
                if (!node) return null;
                var clone = { id: String(node.id), text: node.text, items: [], hasChildren: !!(node.items && node.items.length) };
                (node.items || []).forEach(function(child){
                    if (child.hasChildren) {
                        var g = cloneSubtree(child);
                        if (g) clone.items.push(g);
                    } else {
                        clone.items.push({ id: String(child.id), text: child.text, hasChildren: false });
                    }
                });
                return clone;
            }
            (chosen||[]).forEach(function(c){
                var code=c.CODE, name=c.NAME;
                var codeStr = String(code);
                var nodeIfGroup = groupIndex[codeStr];
                var paths = findPathsInData(treeData, codeStr);
                // If the chosen code is a group, clone its subtree directly into the result
                if (nodeIfGroup && nodeIfGroup.hasChildren) {
                    var gclone = cloneSubtree(nodeIfGroup);
                    if (gclone) {
                        // merge into pathMap without duplicating existing roots
                        var existing = pathMap[gclone.id];
                        if (!existing) {
                            pathMap[gclone.id] = gclone;
                        } else {
                            // merge children (dedupe by id)
                            var byId = {};
                            (existing.items||[]).forEach(function(it){ byId[String(it.id)] = it; });
                            (gclone.items||[]).forEach(function(it){ if (!byId[String(it.id)]) existing.items.push(it); });
                            existing.hasChildren = existing.items && existing.items.length>0;
                        }
                        return; // done for this chosen entry
                    }
                }
                if (!paths || paths.length===0){
                    // Attach under the masters the user actually checked
                    if (checkedMasters.length > 0) {
                        checkedMasters.forEach(function(mid){
                            var src = groupIndex[String(mid)];
                            if (!src) return;
                            var g = ensureGroupNode(String(src.id), src.text);
                            addLeafToGroup(g, code, name);
                        });
                    } else {
                        var loose=ensureGroupNode('__loose__','Ungrouped'); addLeafToGroup(loose, code, name);
                    }
                    return;
                }
                paths.forEach(function(path){
                    var current=null;
                    for (var i=0;i<path.length;i++){
                        var node=path[i];
                        var isLeaf=(i===path.length-1);
                        if (isLeaf){
                            if (node && node.hasChildren) {
                                // Chosen code is a group at end of path; copy its subtree under current
                                var sub = cloneSubtree(node);
                                if (current && sub) {
                                    // attach entire subtree
                                    current.items = current.items || [];
                                    current.items.push(sub);
                                    current.hasChildren = true;
                                } else if (sub) {
                                    // no current parent, add as root
                                    pathMap[sub.id] = pathMap[sub.id] || sub;
                                }
                            } else {
                                if (current) addLeafToGroup(current, code, name);
                            }
                        }
                        else { var g=ensureGroupNode(node.id, node.text); if (current && current!==g){ if ((current.items||[]).indexOf(g)===-1) current.items.push(g); current.hasChildren=true; } current=g; }
                    }
                });
            });
            var roots=[], all=Object.values(pathMap), childIds=new Set();
            all.forEach(function(n){ (n.items||[]).forEach(function(ch){ childIds.add(ch.id); }); });
            all.forEach(function(n){ if (!childIds.has(n.id)) roots.push(n); });
            return roots;
        }

        // Find all possible paths to a code inside a TreeView, similar to RateSchedule implementation
        function findPathsInTreeByCode(treeView, code) {
            if (!treeView || !code) return [];
            var view = treeView.dataSource.view();
            var paths = [];
            function dfs(nodes, trail) {
                for (var i = 0; i < nodes.length; i++) {
                    var n = nodes[i];
                    var thisTrail = trail.concat([n]);
                    var isMatch = (n.id === code) || (n.originalCode && n.originalCode === code);
                    if (isMatch) { paths.push(thisTrail); }
                    var children = n.hasChildren && n.items ? n.items : [];
                    if (children.length) dfs(children, thisTrail);
                }
            }
            dfs(view, []);
            return paths;
        }

        // Build a hierarchy using the exact lineage in the source tree, like RateSchedule's buildSpecificPathTreeFromIndividuals
        function buildSpecificPathTreeFromIndividuals(treeSelector, chosen) {
            var tv = $(treeSelector).data("kendoTreeView");
            var pathMap = {};
            var ensureGroupNode = function(id, text) {
                if (!pathMap[id]) {
                    pathMap[id] = { id: id, text: text, items: [], hasChildren: false, _leafSet: {} };
                } else if (!pathMap[id]._leafSet) {
                    pathMap[id]._leafSet = {};
                }
                return pathMap[id];
            };

            // Helper to add a leaf under a group, deduplicated
            var addLeafToGroup = function(groupNode, code, name) {
                groupNode.items = groupNode.items || [];
                groupNode.hasChildren = true;
                if (!groupNode._leafSet) groupNode._leafSet = {};
                if (!groupNode._leafSet[code]) {
                    groupNode._leafSet[code] = true;
                    groupNode.items.push({ id: code, text: name, hasChildren: false });
                }
            };

            // Determine currently checked groups in the source tree (fallback lineage)
            var checkedGroups = [];
            if (tv && tv.dataSource && typeof tv.dataSource.flatView === 'function') {
                // Method 1: Try to get checked nodes from data model
                var flat = tv.dataSource.flatView();
                checkedGroups = flat.filter(function(n){
                    return n && n.checked && (n.hasChildren === true || (Array.isArray(n.items) && n.items.length > 0));
                });
                
                // Method 2: If no checked nodes found in data model, inspect DOM directly
                if (checkedGroups.length === 0) {
                    console.log('[DEBUG] DiscountSchedule: No checked groups in data model, inspecting DOM...');
                    try {
                        tv.element.find('input[type="checkbox"]:checked').each(function(){
                            var $item = $(this).closest('.k-item');
                            var dataItem = tv.dataItem($item);
                            if (dataItem && (dataItem.hasChildren === true || (Array.isArray(dataItem.items) && dataItem.items.length > 0))) {
                                checkedGroups.push(dataItem);
                            }
                        });
                        console.log('[DEBUG] DiscountSchedule: Found checked groups via DOM:', checkedGroups.length);
                    } catch(e) {
                        console.warn('[DEBUG] DiscountSchedule: DOM inspection failed:', e);
                    }
                }
            }

            (chosen || []).forEach(function (c) {
                var code = c.CODE;
                var name = c.NAME;
                var paths = findPathsInTreeByCode(tv, code);
                if (!paths || paths.length === 0) {
                    // Fallback: attach under each currently checked group
                    if (checkedGroups.length > 0) {
                        checkedGroups.forEach(function(g){
                            var gnode = ensureGroupNode(g.id, g.text || g.NAME || '');
                            addLeafToGroup(gnode, code, name);
                        });
                    } else {
                        // Final fallback when no group context is available
                        var loose = ensureGroupNode('__loose__', 'Ungrouped');
                        addLeafToGroup(loose, code, name);
                    }
                    return;
                }
                paths.forEach(function (path) {
                    var currentParent = null;
                    for (var i = 0; i < path.length; i++) {
                        var node = path[i];
                        var isLeaf = (i === path.length - 1);
                        if (isLeaf) {
                            if (currentParent) {
                                addLeafToGroup(currentParent, code, name);
                            } else {
                                // No parent group in path (unlikely), place under loose
                                var loose = ensureGroupNode('__loose__', 'Ungrouped');
                                addLeafToGroup(loose, code, name);
                            }
                        } else {
                            var gnode = ensureGroupNode(node.id, node.text);
                            if (currentParent && currentParent !== gnode) {
                                currentParent.items = currentParent.items || [];
                                // Avoid duplicate linking
                                if (currentParent.items.indexOf(gnode) === -1) {
                                    currentParent.items.push(gnode);
                                }
                                currentParent.hasChildren = true;
                            }
                            currentParent = gnode;
                        }
                    }
                });
            });
            // Collect roots (avoid duplicating '__loose__')
            var roots = [];
            var allNodes = Object.values(pathMap);
            var childIds = new Set();
            allNodes.forEach(function(node) {
                (node.items || []).forEach(function(child){ childIds.add(child.id); });
            });
            allNodes.forEach(function(node){ if (!childIds.has(node.id)) roots.push(node); });
            return roots;
        }

        function renderSelectedTree(hierData) {
            var treeId = getSelectedTreeElementId();
            var existing = $(treeId).data('kendoTreeView');
            if (existing) { existing.destroy(); $(treeId).empty(); }
            var data = hierData && hierData.length ? hierData : buildSelectedTreeData();
            $(treeId).kendoTreeView({
                dataSource: new kendo.data.HierarchicalDataSource({ data: data }),
                dataTextField: 'text',
                checkboxes: { checkChildren: true, threeState: false },
                loadOnDemand: false
            });
            // Expand all nodes for full visibility
            var tv = $(treeId).data('kendoTreeView');
            if (tv) {
                $(treeId).find(".k-item").each(function(){
                    try { tv.expand(this); } catch(e) {}
                });
            }
        }

        // Utilities
        function partyTypeToCsFlag() {
            switch (($scope.discountSchedule.partyType || '').toLowerCase()) {
                case 'customer': return 'C';
                case 'dealer': return 'P';
                case 'supplier': return 'S';
                default: return 'C';
            }
        }

        function updateItemsSummary() {
            var chosen = ($scope.selectedItems || []).length;
            $scope.itemsSummary = chosen > 1 ? (chosen + ' items selected') : (chosen === 1 ? ($scope.selectedItems[0].ITEM_EDESC || '') : '');
        }

        // Load dropdowns
        function loadBasics(){
            $http.get('/api/SetupApi/GetCurrencyListForRateSchedule').then(function(resp){
                $scope.currencyList = (resp.data && (resp.data.DATA || resp.data.Data)) || [];
            });
            $http.get('/api/SetupApi/GetCharges').then(function(resp){
                var raw = (resp.data && (resp.data.DATA || resp.data.Data)) || (Array.isArray(resp.data) ? resp.data : []);
                $scope.chargeCodeList = (raw || []).map(function(r){
                    return {
                        CHARGE_CODE: r.CHARGE_CODE || r.ChargeCode || r.code || r.CHARGE_ID,
                        CHARGE_EDESC: r.CHARGE_EDESC || r.Description || r.CHARGE_NDESC || r.name || r.CHARGE_NAME
                    };
                }).filter(function(x){ return !!x.CHARGE_CODE; });
            });
            $http.get('/api/SetupApi/GetDocumentList').then(function(resp){
                $scope.documentList = (resp.data && (resp.data.DATA || resp.data.Data)) || [];
            });
        }
        loadBasics();

        // Modals
        $scope.openCustomerModal = function(){ openParty('customer'); };
        $scope.openDealerModal = function(){ openParty('dealer'); };
        $scope.openSupplierModal = function(){ openParty('supplier'); };
        $scope.openItemModal = function(){ openItems(); };

        function openParty(type) {
            debugger
            $scope.discountSchedule.partyType = type;
            if (!window.PartyItemSelector || !PartyItemSelector.openPartyModal) return;
            // Invalidate cached full tree to ensure latest lineage is used
            $scope._fullTreeByParty[type] = null;
            PartyItemSelector.openPartyModal(type, {}).then(function(result){
                var rows = Array.isArray(result && result.data) ? result.data : [];
                var checkedMasters = (result && result.checkedMasterIds) || [];

                function normalizeToParties(list){
                    return (list||[]).map(function(r){
                        var code = r.CUSTOMER_CODE || r.PARTY_TYPE_CODE || r.SUPPLIER_CODE || r.CODE || r.id || r.MASTER_CUSTOMER_CODE || r.MASTER_PARTY_CODE || r.MASTER_SUPPLIER_CODE;
                        var name = r.CUSTOMER_EDESC || r.PARTY_TYPE_EDESC || r.SUPPLIER_EDESC || r.NAME || r.DESCRIPTION || r.ITEM_EDESC;
                        return { CODE: code, NAME: name };
                    }).filter(function(x){ return !!x.CODE; });
                }

                var finalizeRender = function(){
                    $scope.discountSchedule.partyName = $scope.selectedParties.length === 1 ? $scope.selectedParties[0].NAME : ($scope.selectedParties.length + ' selected');
                    $scope.discountSchedule.partyCode = $scope.selectedParties.length === 1 ? $scope.selectedParties[0].CODE : '';
                    ensureFullTreeWithIndividuals(type).then(function(fullTree){
                        var hier = buildSpecificPathTreeFromData(fullTree, $scope.selectedParties, checkedMasters);
                        renderSelectedTree(hier);
                        $scope.$applyAsync();
                    });
                };

                // Case 1: individuals selected in grid -> use them directly
                if (rows && rows.length > 0) {
                    $scope.selectedParties = normalizeToParties(rows);
                    return finalizeRender();
                }

                // Case 2: no individual rows selected but groups were checked -> fetch individuals for groups
                if (checkedMasters && checkedMasters.length > 0) {
                    var batchUrl = '/api/SetupApi/GetIndividualsByMasterCodes';
                    var csFlag = (type === 'customer') ? 'C' : (type === 'dealer') ? 'P' : 'S';
                    $http.post(batchUrl, { PartyType: type, CsFlag: csFlag, MasterCodes: checkedMasters }, { headers: { 'Content-Type': 'application/json' } })
                        .then(function(resp){
                            var payload = (resp.data && (resp.data.DATA || resp.data.Data)) || (Array.isArray(resp.data) ? resp.data : []);
                            $scope.selectedParties = normalizeToParties(payload);
                            finalizeRender();
                        })
                        .catch(function(){
                            // Fallback: per-group GET
                            var apiUrl;
                            if (type === 'customer') apiUrl = '/api/SetupApi/GetChildOfCustomerByGroup';
                            else if (type === 'dealer') apiUrl = '/api/SetupApi/GetChildOfDealerByGroup';
                            else if (type === 'supplier') apiUrl = '/api/SetupApi/GetChildOfsupplierByGroup';
                            var reqs = checkedMasters.map(function(mc){ return $http.get(apiUrl + '?groupCode=' + encodeURIComponent(mc)); });
                            Promise.all(reqs).then(function(resps){
                                var merged = [];
                                (resps||[]).forEach(function(resp){
                                    var payload = Array.isArray(resp.data) ? resp.data : (resp.data && (resp.data.DATA || resp.data.Data)) || [];
                                    merged = merged.concat(payload || []);
                                });
                                $scope.selectedParties = normalizeToParties(merged);
                                finalizeRender();
                            }).catch(function(){
                                $scope.selectedParties = [];
                                finalizeRender();
                            });
                        });
                    return;
                }

                // Default: nothing selected
                $scope.selectedParties = [];
                finalizeRender();
            });
        }

        function openItems(){
            if (!window.PartyItemSelector || !PartyItemSelector.openItemModal) return;
            PartyItemSelector.openItemModal({}).then(function(result){
                var rows = Array.isArray(result && result.data) ? result.data : [];
                var checkedMasters = (result && result.checkedMasterIds) || [];
                // Normalize to { ITEM_CODE, ITEM_EDESC }
                var items = rows.map(function(r){
                    return {
                        ITEM_CODE: r.ITEM_CODE || r.CODE || r.id,
                        ITEM_EDESC: r.ITEM_EDESC || r.NAME || r.DESCRIPTION || r.text,
                        PRE_ITEM_CODE: r.PRE_ITEM_CODE || r.preItemCode || r.PRE_CODE || r.MASTER_ITEM_CODE || r.masterItemCode // fallback if API returns master on individuals
                    };
                }).filter(function(x){ return !!x.ITEM_CODE; });

                // If no individual rows selected but groups were checked, fetch individuals per group
                if ((!items || items.length === 0) && checkedMasters && checkedMasters.length > 0) {
                    var reqs = checkedMasters.map(function(mc){ return $http.get('/api/SetupApi/GetItemsByGroup?groupCode=' + encodeURIComponent(mc)); });
                    return Promise.all(reqs).then(function(resps){
                        var merged = [];
                        (resps||[]).forEach(function(resp){
                            var payload = Array.isArray(resp.data) ? resp.data : (resp.data && (resp.data.DATA || resp.data.Data)) || [];
                            merged = merged.concat(payload || []);
                        });
                        var norm = (merged||[]).map(function(r){
                            return {
                                ITEM_CODE: r.ITEM_CODE || r.CODE || r.id,
                                ITEM_EDESC: r.ITEM_EDESC || r.NAME || r.DESCRIPTION || r.text,
                                PRE_ITEM_CODE: r.PRE_ITEM_CODE || r.preItemCode || r.PRE_CODE || r.MASTER_ITEM_CODE || r.masterItemCode
                            };
                        }).filter(function(x){ return !!x.ITEM_CODE; });
                        $scope.selectedItems = norm;
                        updateItemsSummary();
                        return finalizeItemPreparation();
                    }).catch(function(){
                        $scope.selectedItems = [];
                        updateItemsSummary();
                        return finalizeItemPreparation();
                    });
                }

                $scope.selectedItems = items;
                updateItemsSummary();
                // Fetch MU and latest discount in batch
                var codes = items.map(function(i){ return i.ITEM_CODE; });
                if (codes.length) {
                    var muReq = $http.get('/api/SetupApi/GetMuDescriptionBatch?itemCodes=' + encodeURIComponent(codes.join(',')));
                    var csFlag = partyTypeToCsFlag();
                    var eff = $scope.discountSchedule.effectiveDate ? moment($scope.discountSchedule.effectiveDate).format('YYYY-MM-DD') : '';
                    var discReq = $http.get('/api/SetupApi/GetItemDiscountsByDate?itemCodes=' + encodeURIComponent(codes.join(',')) + '&csFlag=' + encodeURIComponent(csFlag) + (eff ? ('&effectiveDate=' + encodeURIComponent(eff)) : ''));
                    // Also fetch item group names to support grouping UI
                    var groupReq = $http.get('/api/SetupApi/GetItemGroups');
                    Promise.all([muReq, discReq, groupReq]).then(function(responses){
                        var muResp = responses[0];
                        var discResp = responses[1];
                        var grpResp = responses[2];
                        var muData = (muResp.data && (muResp.data.DATA || muResp.data.Data)) || [];
                        var discData = (discResp.data && (discResp.data.DATA || discResp.data.Data)) || [];
                        var groupRows = Array.isArray(grpResp.data) ? grpResp.data : (grpResp.data && (grpResp.data.DATA || grpResp.data.Data)) || [];
                        // Build maps from group payload
                        // 1) Group master -> Group name (for parent name lookup)
                        var groupNameByMaster = {};
                        // 2) Item code -> PRE (parent master) code (for reliable parent resolution)
                        var preByItem = {};
                        (groupRows || []).forEach(function(r){
                            var isGroup = (r.GROUP_SKU_FLAG === 'G') || (r.GROUP_SKU_FLAG === 'g');
                            var masterKey = r.MASTER_ITEM_CODE || r.MASTER_CODE || r.id;
                            var name = r.ITEM_EDESC || r.EDESC || r.NAME;
                            if (isGroup && masterKey) groupNameByMaster[String(masterKey)] = name || String(masterKey);
                            var itemCode = r.ITEM_CODE || r.CODE || r.id;
                            var preCode = r.PRE_ITEM_CODE || r.PRE_CODE || r.MASTER_CODE || r.preItemCode || r.masterItemCode;
                            if (itemCode != null && preCode != null) preByItem[String(itemCode)] = String(preCode);
                        });
                        $scope._itemGroupNameByMaster = groupNameByMaster;
                        $scope._preByItem = preByItem;

                        // If user had checked master groups in the modal, fetch their children to ensure parent mapping
                        var ensureParentMapPromise = Promise.resolve();
                        if (checkedMasters && checkedMasters.length > 0) {
                            var requests = (checkedMasters || []).map(function(mc){
                                var url = '/api/SetupApi/GetItemsByGroup?groupCode=' + encodeURIComponent(mc);
                                return $http.get(url).then(function(resp){
                                    var list = Array.isArray(resp.data) ? resp.data : (resp.data && (resp.data.DATA || resp.data.Data)) || [];
                                    (list || []).forEach(function(r){
                                        var ic = r.ITEM_CODE || r.CODE || r.id;
                                        var pre = r.PRE_ITEM_CODE || r.PRE_CODE || r.MASTER_CODE || r.preItemCode || r.masterItemCode || mc; // fallback to the group master
                                        if (ic != null && pre != null) $scope._preByItem[String(ic)] = String(pre);
                                    });
                                }).catch(function(){});
                            });
                            ensureParentMapPromise = Promise.all(requests);
                        }
                        var firstMuByItem = {};
                        muData.forEach(function(row){ if (row.ITEM_CODE && row.MU_CODE && firstMuByItem[row.ITEM_CODE] == null) firstMuByItem[row.ITEM_CODE] = row.MU_CODE; });
                        var discByItem = {};
                        discData.forEach(function(row){ if (row.ITEM_CODE) discByItem[row.ITEM_CODE] = row; });
                        ensureParentMapPromise.then(function(){
                            $scope.selectedItems.forEach(function(i){
                                i.MU_CODE = i.MU_CODE || firstMuByItem[i.ITEM_CODE] || '';
                                var d = discByItem[i.ITEM_CODE];
                                if (d) {
                                    i._prefill = {
                                        MU_CODE: d.MU_CODE || i.MU_CODE || '',
                                        DISCOUNT_RATE: d.DISCOUNT_RATE||0,
                                        DISCOUNT_PERCENT: d.DISCOUNT_PERCENT||0,
                                        ITEM_DISCOUNT_RATE: d.ITEM_DISCOUNT_RATE||0,
                                        ITEM_DISCOUNT_PERCENT: d.ITEM_DISCOUNT_PERCENT||0
                                    };
                                }
                                // Assign parent item description for grouping purpose (prefer mapping from groups payload and checked masters children)
                                var parentKey = ($scope._preByItem && $scope._preByItem[String(i.ITEM_CODE)]) ? $scope._preByItem[String(i.ITEM_CODE)] : (i.PRE_ITEM_CODE != null ? String(i.PRE_ITEM_CODE) : null);
                                i.PARENT_EDESC = parentKey && $scope._itemGroupNameByMaster ? ($scope._itemGroupNameByMaster[parentKey] || 'Ungrouped') : 'Ungrouped';
                            });
                            buildDiscountGrid();
                        }).catch(function(){ buildDiscountGrid(); });
                    }).catch(function(){ buildDiscountGrid(); });
                } else {
                    buildDiscountGrid();
                }
                $scope.$applyAsync();
            });
        }

        function finalizeItemPreparation(){
            var items = $scope.selectedItems || [];
            var codes = items.map(function(i){ return i.ITEM_CODE; });
            if (!codes.length) { buildDiscountGrid(); $scope.$applyAsync(); return Promise.resolve(); }
            var muReq = $http.get('/api/SetupApi/GetMuDescriptionBatch?itemCodes=' + encodeURIComponent(codes.join(',')));
            var csFlag = partyTypeToCsFlag();
            var eff = $scope.discountSchedule.effectiveDate ? moment($scope.discountSchedule.effectiveDate).format('YYYY-MM-DD') : '';
            var discReq = $http.get('/api/SetupApi/GetItemDiscountsByDate?itemCodes=' + encodeURIComponent(codes.join(',')) + '&csFlag=' + encodeURIComponent(csFlag) + (eff ? ('&effectiveDate=' + encodeURIComponent(eff)) : ''));
            var groupReq = $http.get('/api/SetupApi/GetItemGroups');
            return Promise.all([muReq, discReq, groupReq]).then(function(responses){
                var muResp = responses[0], discResp = responses[1], grpResp = responses[2];
                var muData = (muResp.data && (muResp.data.DATA || muResp.data.Data)) || [];
                var discData = (discResp.data && (discResp.data.DATA || discResp.data.Data)) || [];
                var groupRows = Array.isArray(grpResp.data) ? grpResp.data : (grpResp.data && (grpResp.data.DATA || grpResp.data.Data)) || [];
                var groupNameByMaster = {};
                (groupRows || []).forEach(function(r){
                    var isGroup = (r.GROUP_SKU_FLAG === 'G') || (r.GROUP_SKU_FLAG === 'g');
                    var key = r.MASTER_ITEM_CODE || r.MASTER_CODE || r.id;
                    var name = r.ITEM_EDESC || r.EDESC || r.NAME;
                    if (isGroup && key) groupNameByMaster[String(key)] = name || String(key);
                });
                $scope._itemGroupNameByMaster = groupNameByMaster;
                var firstMuByItem = {};
                muData.forEach(function(row){ if (row.ITEM_CODE && row.MU_CODE && firstMuByItem[row.ITEM_CODE] == null) firstMuByItem[row.ITEM_CODE] = row.MU_CODE; });
                var discByItem = {};
                discData.forEach(function(row){ if (row.ITEM_CODE) discByItem[row.ITEM_CODE] = row; });
                $scope.selectedItems.forEach(function(i){
                    i.MU_CODE = i.MU_CODE || firstMuByItem[i.ITEM_CODE] || '';
                    var d = discByItem[i.ITEM_CODE];
                    if (d) {
                        i._prefill = {
                            MU_CODE: d.MU_CODE || i.MU_CODE || '',
                            DISCOUNT_RATE: d.DISCOUNT_RATE||0,
                            DISCOUNT_PERCENT: d.DISCOUNT_PERCENT||0,
                            ITEM_DISCOUNT_RATE: d.ITEM_DISCOUNT_RATE||0,
                            ITEM_DISCOUNT_PERCENT: d.ITEM_DISCOUNT_PERCENT||0
                        };
                    }
                    // Assign parent item description for grouping purpose
                    var parentKey = i.PRE_ITEM_CODE != null ? String(i.PRE_ITEM_CODE) : null;
                    i.PARENT_EDESC = parentKey && $scope._itemGroupNameByMaster ? ($scope._itemGroupNameByMaster[parentKey] || 'Ungrouped') : 'Ungrouped';
                });
                buildDiscountGrid();
                $scope.$applyAsync();
            }).catch(function(){ buildDiscountGrid(); $scope.$applyAsync(); });
        }

        // Grid
        function buildDiscountGrid() {
            var grid = $('#discountGrid').data('kendoGrid');
            var data = ($scope.selectedItems || []).map(function (i) {
                return {
                    ITEM_CODE: i.ITEM_CODE,
                    ITEM_EDESC: i.ITEM_EDESC,
                    PARENT_EDESC: i.PARENT_EDESC || 'Ungrouped',
                    MU_CODE: (i._prefill && i._prefill.MU_CODE) || i.MU_CODE || '',
                    DISCOUNT_RATE: (i._prefill && i._prefill.DISCOUNT_RATE) || 0,
                    DISCOUNT_PERCENT: (i._prefill && i._prefill.DISCOUNT_PERCENT) || 0,
                    ITEM_DISCOUNT_RATE: (i._prefill && i._prefill.ITEM_DISCOUNT_RATE) || 0,
                    ITEM_DISCOUNT_PERCENT: (i._prefill && i._prefill.ITEM_DISCOUNT_PERCENT) || 0
                };
            });
            if (grid) { grid.dataSource.data(data); return; }
            $('#discountGrid').kendoGrid({
                dataSource: { data: data, schema: { model: { id: 'ITEM_CODE' } }, group: { field: 'PARENT_EDESC' } },
                height: 520,
                scrollable: true,
                editable: true,
                columns: [
                    { field: 'PARENT_EDESC', title: 'Parent Item', width: 220, groupHeaderTemplate: function(d){
                        var parent = (d && d.value) ? d.value : 'Ungrouped';
                        // Render group-level editable inputs to broadcast to all members
                        return '<div class="ds-group-header" data-parent="'+ kendo.htmlEncode(parent) +'">'
                            + '<span class="k-icon k-i-folder"></span> ' + kendo.htmlEncode(parent)
                            + '<div class="pull-right ds-group-inputs">'
                            + '<span title="Discount Rate"><input type="number" step="0.01" class="ds-group-input form-control input-xs" style="width:95px;display:inline-block;margin-left:6px;" data-field="DISCOUNT_RATE" data-parent="'+ kendo.htmlEncode(parent) +'" placeholder="Rate"/></span>'
                            + '<span title="Discount %"><input type="number" step="0.01" class="ds-group-input form-control input-xs" style="width:85px;display:inline-block;margin-left:6px;" data-field="DISCOUNT_PERCENT" data-parent="'+ kendo.htmlEncode(parent) +'" placeholder="%"/></span>'
                            + '<span title="Item Disc Rate"><input type="number" step="0.01" class="ds-group-input form-control input-xs" style="width:110px;display:inline-block;margin-left:6px;" data-field="ITEM_DISCOUNT_RATE" data-parent="'+ kendo.htmlEncode(parent) +'" placeholder="Item Rate"/></span>'
                            + '<span title="Item Disc %"><input type="number" step="0.01" class="ds-group-input form-control input-xs" style="width:100px;display:inline-block;margin-left:6px;" data-field="ITEM_DISCOUNT_PERCENT" data-parent="'+ kendo.htmlEncode(parent) +'" placeholder="Item %"/></span>'
                            + '</div>'
                            + '</div>';
                    } },
                    { field: 'ITEM_CODE', title: 'Item Code', width: 120, editable: false },
                    { field: 'ITEM_EDESC', title: 'Item Description', width: 220, editable: false },
                    { field: 'MU_CODE', title: 'MU', width: 80 },
                    { field: 'DISCOUNT_RATE', title: 'Discount Rate', width: 120, format: '{0:n2}' },
                    { field: 'DISCOUNT_PERCENT', title: 'Discount %', width: 110, format: '{0:n2}' },
                    { field: 'ITEM_DISCOUNT_RATE', title: 'Item Disc Rate', width: 130, format: '{0:n2}' },
                    { field: 'ITEM_DISCOUNT_PERCENT', title: 'Item Disc %', width: 120, format: '{0:n2}' }
                ],
                dataBound: function(){
                    var g = $('#discountGrid').data('kendoGrid');
                    if (!g) return;
                    var headerStore = $scope._groupHeaderValues || ($scope._groupHeaderValues = {});
                    var headerTimers = $scope._groupHeaderTimers || ($scope._groupHeaderTimers = {});
                    // Wire group header inputs to propagate changes to all rows with same parent
                    $('#discountGrid').find('.ds-group-input').off('input.dsGroup').on('input.dsGroup', function(){
                        var $inp = $(this);
                        var field = $inp.data('field');
                        var parent = $inp.data('parent');
                        var raw = ($inp.val() || '').toString();
                        var val = raw === '' ? '' : (isNaN(raw) ? 0 : Number(raw));
                        // Store the typed value so it persists across rebinds
                        if (!headerStore[parent]) headerStore[parent] = {};
                        headerStore[parent][field] = raw;
                        // Preserve caret position and focus
                        var caretStart = this.selectionStart, caretEnd = this.selectionEnd;
                        var timerKey = parent + '::' + field;
                        if (headerTimers[timerKey]) { clearTimeout(headerTimers[timerKey]); }
                        headerTimers[timerKey] = setTimeout(function(){
                            // Iterate all data items (flatten groups) and apply updates
                            function eachItem(groups){
                                (groups||[]).forEach(function(gr){
                                    if (gr.items && gr.items.length && gr.hasSubgroups){
                                        eachItem(gr.items);
                                    } else if (gr.items && gr.items.length){
                                        gr.items.forEach(function(it){ if ((it.PARENT_EDESC || 'Ungrouped') === parent){ it.set ? it.set(field, val) : (it[field] = val); } });
                                    }
                                });
                            }
                            var view = g.dataSource.view();
                            if (g.dataSource.group() && g.dataSource.group().length){ eachItem(view); }
                            else {
                                // Not grouped – simple iteration
                                (g.dataSource.data() || []).forEach(function(it){ if ((it.PARENT_EDESC || 'Ungrouped') === parent){ it.set ? it.set(field, val) : (it[field] = val); } });
                            }
                            // Restore focus and caret
                            setTimeout(function(){
                                var ref = $('#discountGrid').find('.ds-group-input[data-parent="'+ kendo.htmlEncode(parent) +'"][data-field="'+ field +'"]').first();
                                if (ref && ref.length){
                                    ref.focus();
                                    try { ref[0].setSelectionRange(caretStart, caretEnd); } catch(e) {}
                                }
                            }, 0);
                        }, 150); // debounce to reduce reflows
                    });
                    // Restore last typed values into the header inputs so user sees what they typed
                    $('#discountGrid').find('.ds-group-input').each(function(){
                        var $inp = $(this);
                        var field = $inp.data('field');
                        var parent = $inp.data('parent');
                        var stored = headerStore[parent] && headerStore[parent][field];
                        if (stored !== undefined) {
                            $inp.val(stored);
                        }
                    });
                }
            });
        }

        $scope.applyDiscountGridSearch = function () {
            var g = $('#discountGrid').data('kendoGrid');
            if (!g) return;
            var q = ($scope.discountGridSearch || '').toString().trim();
            if (!q) { g.dataSource.filter([]); return; }
            g.dataSource.filter({
                logic: 'or', filters: [
                    { field: 'ITEM_CODE', operator: 'contains', value: q },
                    { field: 'ITEM_EDESC', operator: 'contains', value: q }
                ]
            });
        };

        // Party type change
        $scope.onPartyTypeChange = function () {
            $scope.selectedParties = [];
            $scope.discountSchedule.partyName = '';
            $scope.discountSchedule.partyCode = '';
            // Re-render empty tree for new party type
            renderSelectedTree();
        };

        // Search and selection helpers for the selected tree panel
        $scope.applySelectedTreeSearch = function () {
            var treeId = getSelectedTreeElementId();
            var g = ($scope.selectedParties || []);
            var q = ($scope.selectedTreeSearch || '').toString().trim().toLowerCase();
            var filtered = !q ? g : g.filter(function (p) {
                return (p.CODE && p.CODE.toString().toLowerCase().indexOf(q) !== -1) ||
                    (p.NAME && p.NAME.toString().toLowerCase().indexOf(q) !== -1);
            });
            var tv = $(treeId).data('kendoTreeView');
            if (tv) { tv.destroy(); $(treeId).empty(); }
            $(treeId).kendoTreeView({
                dataSource: new kendo.data.HierarchicalDataSource({
                    data: (filtered || []).map(function (p) { return { id: p.CODE, text: p.NAME || p.CODE, hasChildren: false }; })
                }),
                dataTextField: 'text',
                checkboxes: { checkChildren: false, threeState: false },
                loadOnDemand: false
            });
        };

        $scope.selectAllSelectedTree = function () {
            var treeId = getSelectedTreeElementId();
            var tv = $(treeId).data('kendoTreeView');
            if (!tv) return;
            $(treeId).find('.k-item').each(function () {
                var item = $(this);
                var cb = item.find('input[type=checkbox]');
                if (cb && !cb.prop('checked')) { cb.prop('checked', true).trigger('change'); }
            });
        };

        $scope.clearAllSelectedTree = function () {
            var treeId = getSelectedTreeElementId();
            var tv = $(treeId).data('kendoTreeView');
            if (!tv) return;
            $(treeId).find('input[type=checkbox]').prop('checked', false).trigger('change');
        };

        // Save
        $scope.saveDiscountSchedule = function () {
            debugger
            // notifier consistent with RateSchedule
            var notify = function (msg, type) {
                try {
                    if (typeof window.displayPopupNotification === 'function') {
                        window.displayPopupNotification(msg, type || 'success');
                    } else {
                        alert(msg);
                    }
                } catch (e) { alert(msg); }
            };

            var grid = $('#discountGrid').data('kendoGrid');
            var rows = [];
            try {
                if (grid && grid.saveChanges) grid.saveChanges();
                var ds = grid && grid.dataSource && grid.dataSource.data ? grid.dataSource.data() : [];
                if (ds && typeof ds.toJSON === 'function') rows = ds.toJSON(); else rows = ds || [];
            } catch (eSync) { rows = []; }

            // Basic validations
            if (!$scope.discountSchedule.effectiveDate) { notify('Please select an effective date.', 'warning'); return; }
            // Collect ONLY the checked leaf nodes from the Selected <Party> tree
            var partyCodes = [];
            try {
                var treeId = getSelectedTreeElementId();
                var tvSel = $(treeId).data('kendoTreeView');
                if (tvSel) {
                    // Use DOM to reliably get checked boxes, then map to dataItems and include only leaves
                    $(treeId).find('.k-item input[type="checkbox"]:checked').each(function(){
                        var $li = $(this).closest('.k-item');
                        var dataItem = tvSel.dataItem($li);
                        if (!dataItem) return;
                        var isLeaf = !(dataItem.hasChildren === true || (Array.isArray(dataItem.items) && dataItem.items.length > 0));
                        if (!isLeaf) return; // skip group nodes
                        var code = dataItem.id || dataItem.CODE || dataItem.code;
                        if (code != null) partyCodes.push(String(code));
                    });
                }
                if ((!partyCodes || partyCodes.length === 0) && $scope.selectedParties && $scope.selectedParties.length) {
                    // Fallback to legacy behavior if tree not present or no checkboxes found
                    partyCodes = ($scope.selectedParties || []).map(function (p) { return p && (p.CODE || p.code || p.id); }).filter(Boolean);
                }
            } catch(e) {
                partyCodes = ($scope.selectedParties || []).map(function (p) { return p && (p.CODE || p.code || p.id); }).filter(Boolean);
            }
            if (!partyCodes.length) { notify('Please select at least one party (individual).', 'warning'); return; }
            if (!rows || rows.length === 0) { notify('Please add at least one item to Discounts grid.', 'warning'); return; }

            // Build payload
            var eff = $scope.discountSchedule.effectiveDate;
            var effStr = (window.moment && eff) ? moment(eff).format('YYYY-MM-DD') : eff;
            var payload = {
                EffectiveDate: effStr,
                CurrencyCode: $scope.discountSchedule.currencyCode || 'NRS',
                ExchangeRate: Number($scope.discountSchedule.exchangeRate || 1),
                // AreaCode removed from UI; send empty
                AreaCode: '',
                DocumentCode: $scope.discountSchedule.documentCode || '',
                ChargeCode: $scope.discountSchedule.chargeCode || '',
                CsFlag: partyTypeToCsFlag(),
                PartyCodes: partyCodes,
                DiscountData: (rows || []).map(function (r) {
                    return {
                        ITEM_CODE: r.ITEM_CODE,
                        MU_CODE: r.MU_CODE || '',
                        DISCOUNT_RATE: Number(r.DISCOUNT_RATE || 0),
                        DISCOUNT_PERCENT: Number(r.DISCOUNT_PERCENT || 0),
                        ITEM_DISCOUNT_RATE: Number(r.ITEM_DISCOUNT_RATE || 0),
                        ITEM_DISCOUNT_PERCENT: Number(r.ITEM_DISCOUNT_PERCENT || 0)
                    };
                })
            };

            $http.post('/api/SetupApi/SaveDiscountSchedule', payload)
                .then(function (resp) {
                    var ok = resp && resp.data && (resp.data.STATUS_CODE === 200 || resp.data.StatusCode === 200);
                    var msg = (resp && resp.data && (resp.data.MESSAGE || resp.data.Message)) || ('Discount Schedule saved for ' + partyCodes.length + ' party(ies).');
                    if (ok) notify(msg, 'success'); else notify(msg || 'Failed to save discount schedule.', 'error');
                })
                .catch(function (err) {
                    var emsg = (err && err.data && (err.data.MESSAGE || err.data.Message)) || 'Failed to save discount schedule.';
                    notify(emsg, 'error');
                });
        };
    });
})();
