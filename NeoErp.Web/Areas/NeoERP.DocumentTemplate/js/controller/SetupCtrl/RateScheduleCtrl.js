DTModule.controller('RateScheduleCtrl', function ($scope, $http, $timeout) {
    $scope.getCheckedNodesInfo = function (treeView) {
        var list = [];
        if (!treeView) return list;
        
        // Method 1: Try to get checked nodes from data model
        var flat = (treeView && treeView.dataSource && typeof treeView.dataSource.flatView === 'function') ? treeView.dataSource.flatView() : [];
        var checked = flat.filter(function (n) { return n && n.checked; });
        
        // Method 2: If no checked nodes found in data model, inspect DOM directly
        if (checked.length === 0) {
            console.log('[DEBUG] RateSchedule: No checked nodes in data model, inspecting DOM...');
            treeView.element.find('input[type=checkbox]:checked').each(function () {
                var item = $(this).closest('.k-item');
                var di = treeView.dataItem(item);
                if (di) {
                    checked.push(di);
                }
            });
            console.log('[DEBUG] RateSchedule: Found checked nodes via DOM:', checked.length);
        }
        
        // Process checked nodes
        checked.forEach(function (di) {
            if (di) {
                var hasChildren = (di.hasChildren === true) || (Array.isArray(di.items) && di.items.length > 0);
                list.push({ id: di.id, text: di.text, isLeaf: !hasChildren, originalCode: di.originalCode });
            }
        });
        
        // De-duplicate
        var map = {}, res = [];
        for (var i = 0; i < list.length; i++) {
            var k = list[i].id;
            if (!map[k]) { map[k] = true; res.push(list[i]); }
        }
        
        console.log('[DEBUG] RateSchedule getCheckedNodesInfo result:', res.map(function(r){ return {id: r.id, text: r.text, isLeaf: r.isLeaf}; }));
        return res;
    };

    $scope.$watch('customerSelectMode', function (nv, ov) {
        $scope.modalState.customer.selectMode = nv;
        if (nv === 'individual') { $scope.currentPartyType = 'customer'; $scope.refreshGenericSelectedGrid(); }
    });
    $scope.$watch('dealerSelectMode', function (nv, ov) {
        $scope.modalState.dealer.selectMode = nv;
        if (nv === 'individual') { $scope.currentPartyType = 'dealer'; $scope.refreshGenericSelectedGrid(); }
    });
    $scope.$watch('supplierSelectMode', function (nv, ov) {
        $scope.modalState.supplier.selectMode = nv;
        if (nv === 'individual') { $scope.currentPartyType = 'supplier'; $scope.refreshGenericSelectedGrid(); }
    });

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

    function buildGroupedTreeFromIndividuals(treeSelector, chosen) {
        var tv = $(treeSelector).data("kendoTreeView");
        var groupMap = {};
        (chosen || []).forEach(function (c) {
            var code = c.CODE;
            var name = c.NAME;
            var paths = findPathsInTreeByCode(tv, code);
            if (!paths || paths.length === 0) {
                // Fallback: add as loose leaf if lineage unknown
                if (!groupMap['__loose__']) groupMap['__loose__'] = { id: '__loose__', text: 'Ungrouped', items: [], _leafSet: {} };
                if (!groupMap['__loose__']._leafSet[code]) {
                    groupMap['__loose__']._leafSet[code] = true;
                    groupMap['__loose__'].items.push({ id: code, text: name, hasChildren: false });
                }
                return;
            }
            paths.forEach(function (path) {
                var keyPath = [];
                for (var i = 0; i < path.length - 1; i++) {
                    var g = path[i];
                    keyPath.push(g.id);
                    var fullKey = keyPath.join('>');
                    if (!groupMap[fullKey]) {
                        groupMap[fullKey] = { id: g.id, text: g.text, items: [], _leafSet: {} };
                        if (i > 0) {
                            var parentKey = keyPath.slice(0, -1).join('>');
                            if (!groupMap[parentKey]) {
                                var pg = path[i - 1];
                                groupMap[parentKey] = { id: pg.id, text: pg.text, items: [], _leafSet: {} };
                            }
                            groupMap[parentKey].items.push(groupMap[fullKey]);
                        }
                    }
                }
                var lastGroupKey = keyPath.join('>');
                var leaf = path[path.length - 1];
                if (!groupMap[lastGroupKey]) {
                    if (!groupMap['__loose__']) groupMap['__loose__'] = { id: '__loose__', text: 'Ungrouped', items: [], _leafSet: {} };
                    if (!groupMap['__loose__']._leafSet[leaf.id]) {
                        groupMap['__loose__']._leafSet[leaf.id] = true;
                        groupMap['__loose__'].items.push({ id: leaf.id, text: leaf.text, hasChildren: false });
                    }
                } else {
                    if (!groupMap[lastGroupKey]._leafSet[leaf.id]) {
                        groupMap[lastGroupKey]._leafSet[leaf.id] = true;
                        groupMap[lastGroupKey].items.push({ id: leaf.id, text: leaf.text, hasChildren: false });
                    }
                }
            });
        });
        var roots = [];
        var keys = Object.keys(groupMap).filter(function (k) { return k !== '__loose__'; });
        var rootKeys = keys.filter(function (k) { return k.indexOf('>') === -1; });
        rootKeys.forEach(function (rk) { roots.push(groupMap[rk]); });
        if (groupMap['__loose__'] && groupMap['__loose__'].items.length) roots = roots.concat(groupMap['__loose__']);
        return roots;
    }

    function buildSpecificPathTreeFromIndividuals(treeSelector, chosen) {
        var tv = $(treeSelector).data("kendoTreeView");
        var pathMap = {};
        
        (chosen || []).forEach(function (c) {
            var code = c.CODE;
            var name = c.NAME;
            var paths = findPathsInTreeByCode(tv, code);
            
            if (!paths || paths.length === 0) {
                if (!pathMap['__loose__']) pathMap['__loose__'] = { id: '__loose__', text: 'Ungrouped', items: [] };
                pathMap['__loose__'].items.push({ id: code, text: name, hasChildren: false });
                return;
            }
            
            paths.forEach(function (path) {
                var currentParent = null;
                var rootNode = null;
                
                for (var i = 0; i < path.length; i++) {
                    var node = path[i];
                    var isLeaf = (i === path.length - 1);
                    
                    if (isLeaf) {
                        if (currentParent) {
                            currentParent.items = currentParent.items || [];
                            currentParent.items.push({ id: node.id, text: name, hasChildren: false });
                            currentParent.hasChildren = true;
                        } else {
                            if (!pathMap[node.id]) {
                                pathMap[node.id] = { id: node.id, text: name, hasChildren: false };
                            }
                        }
                    } else {
                        if (!pathMap[node.id]) {
                            pathMap[node.id] = { id: node.id, text: node.text, items: [], hasChildren: false };
                        }
                        
                        if (currentParent) {
                            currentParent.items = currentParent.items || [];
                            currentParent.items.push(pathMap[node.id]);
                            currentParent.hasChildren = true;
                        } else {
                            rootNode = pathMap[node.id];
                        }
                        
                        currentParent = pathMap[node.id];
                    }
                }
            });
        });
        
        var roots = [];
        var allNodes = Object.values(pathMap);
        var childIds = new Set();
        
        allNodes.forEach(function(node) {
            if (node.items && node.items.length > 0) {
                node.items.forEach(function(child) {
                    childIds.add(child.id);
                });
            }
        });
        
        allNodes.forEach(function(node) {
            if (!childIds.has(node.id)) {
                roots.push(node);
            }
        });
        
        return roots;
    }

    // Initialize data
    $scope.rateSchedule = {
        effectiveDate: new Date(),
        currencyCode: '',
        exchangeRate: 1,
        areaCode: '',
        customerCode: '',
        customerName: '',
        partyType: 'customer',
        partyName: '',
        partyCode: '',
        //optionType: 'percentage', 
        documentCode: '',
        rateData: []
    };

    (function ensureItemsSelectedGridInit() {
        var grid = $("#selectedItemsGrid").data("kendoGrid");
        if (grid) return; 
    })();

 
    (function upgradeSelectedItemsGrid() {
        var grid = $("#selectedItemsGrid").data("kendoGrid");
        if (grid) {
            grid.setOptions({
                columns: [
                    { title: "", width: 40, template: "<input type='checkbox' class='item-row-check' #= selected ? 'checked=checked' : '' # />" },
                    { field: "ITEM_CODE", title: "Item Code", width: "120px" },
                    { field: "ITEM_EDESC", title: "Item Description", width: "200px" },
                    { command: { text: "Remove", click: $scope.removeSelectedItem }, title: "Action", width: "100px" }
                ],
                dataSource: {
                    data: $scope.selectedItems,
                    schema: { model: { id: 'ITEM_CODE', fields: { ITEM_CODE: { type: 'string' }, ITEM_EDESC: { type: 'string' }, selected: { type: 'boolean', defaultValue: false } } } }
                },
                dataBound: function () {
                    var g = this;
                    g.tbody.off('change.items').on('change.items', '.item-row-check', function () {
                        var tr = $(this).closest('tr');
                        var di = g.dataItem(tr);
                        di.set('selected', this.checked);
                    });
                }
            });
        }
    })();

    $scope.filterItemGrid = function (q) {
        var grid = $("#selectedItemsGrid").data("kendoGrid");
        if (!grid) return;
        if (!q) {
            grid.dataSource.filter([]);
            return;
        }
        grid.dataSource.filter({
            logic: 'or',
            filters: [
                { field: 'ITEM_CODE', operator: 'contains', value: q },
                { field: 'ITEM_EDESC', operator: 'contains', value: q }
            ]
        });
    };

    $scope.getCheckedMasterIds = function (treeView) {
        var ids = [];
        if (!treeView) return ids;
        treeView.element.find('input[type=checkbox]:checked').each(function () {
            var item = $(this).closest('.k-item');
            var di = treeView.dataItem(item);
            if (di && di.id) ids.push(di.id);
        });
        var map = {};
        angular.forEach(ids, function (x) { map[x] = true; });
        return Object.keys(map);
    };

    $scope.currencyList = [];
    $scope.areaList = [];
    $scope.documentList = [];
    $scope.customerGroups = [];
    $scope.dealerGroups = [];
    $scope.supplierGroups = [];
    $scope.itemGroups = [];
    $scope.selectedCustomer = null;
    $scope.selectedItems = [];
    $scope.selectedCustomers = $scope.selectedCustomers || [];
    $scope.selectedDealers = $scope.selectedDealers || [];
    $scope.selectedSuppliers = $scope.selectedSuppliers || [];
    $scope.selectedCustomerTreeData = $scope.selectedCustomerTreeData || [];
    $scope.selectedDealerTreeData = $scope.selectedDealerTreeData || [];
    $scope.selectedSupplierTreeData = $scope.selectedSupplierTreeData || [];
    $scope.itemsSummary = '';

    $scope.modalState = $scope.modalState || {
        customer: { checkedIds: [], gridSelectedCodes: [], selectMode: 'group' },
        dealer: { checkedIds: [], gridSelectedCodes: [], selectMode: 'group' },
        supplier: { checkedIds: [], gridSelectedCodes: [], selectMode: 'group' }
    };

    function updateSummaries() {
        if ($scope.rateSchedule.partyType === 'customer') {
            var chosenCustomers = ($scope.customerSelected || []).filter(function (x) { return x.selected; });
            var custCount = chosenCustomers.length;
            if (custCount > 0) {
                $scope.rateSchedule.partyName = custCount === 1 ? (chosenCustomers[0].NAME || '') : (custCount + ' customers selected');
                $scope.rateSchedule.partyCode = custCount === 1 ? (chosenCustomers[0].CODE || '') : '';
            } else {
                $scope.rateSchedule.partyName = '';
                $scope.rateSchedule.partyCode = '';
            }
            $scope.rateSchedule.customerName = $scope.rateSchedule.partyName;
            $scope.rateSchedule.customerCode = $scope.rateSchedule.partyCode;
        } else if ($scope.rateSchedule.partyType === 'dealer') {
            var chosenDealers = ($scope.dealerSelected || []).filter(function (x) { return x.selected; });
            var dealerCount = chosenDealers.length;
            if (dealerCount > 0) {
                $scope.rateSchedule.partyName = dealerCount === 1 ? (chosenDealers[0].NAME || '') : (dealerCount + ' dealers selected');
                $scope.rateSchedule.partyCode = dealerCount === 1 ? (chosenDealers[0].CODE || '') : '';
            } else {
                $scope.rateSchedule.partyName = '';
                $scope.rateSchedule.partyCode = '';
            }
        } else if ($scope.rateSchedule.partyType === 'supplier') {
            var chosenSuppliers = ($scope.supplierSelected || []).filter(function (x) { return x.selected; });
            var supplierCount = chosenSuppliers.length;
            if (supplierCount > 0) {
                $scope.rateSchedule.partyName = supplierCount === 1 ? (chosenSuppliers[0].NAME || '') : (supplierCount + ' suppliers selected');
                $scope.rateSchedule.partyCode = supplierCount === 1 ? (chosenSuppliers[0].CODE || '') : '';
            } else {
                $scope.rateSchedule.partyName = '';
                $scope.rateSchedule.partyCode = '';
            }
        }

        var chosenItems = ($scope.selectedItems || []).filter(function (x) { return x.selected; });
        var itemCount = chosenItems.length;
        if (itemCount > 0) {
            $scope.itemsSummary = itemCount === 1 ? (chosenItems[0].ITEM_EDESC || '') : (itemCount + ' items selected');
        } else {
            $scope.itemsSummary = '';
        }
    }

    $scope.onPartyTypeChange = function () {
        $scope.rateSchedule.partyName = '';
        $scope.rateSchedule.partyCode = '';
        $scope.rateSchedule.customerName = '';
        $scope.rateSchedule.customerCode = '';

        $scope.customerSelected = [];
        $scope.dealerSelected = [];
        $scope.supplierSelected = [];

        updateSummaries();

    };

    function collectAllGroupIds(roots) {
        var ids = [];
        function walk(n) {
            if (!n) return;
            ids.push(n.id);
            (n.items || []).forEach(walk);
        }
        (roots || []).forEach(walk);
        return ids;
    }

    function populateIndividualsForTree(partyType) {
        var cfg = partyConfigs[partyType];
        if (!cfg) return Promise.resolve();
        var treeData = $scope[cfg.selectedTreeDataKey] || [];
        if (!treeData.length) return Promise.resolve();

        var idx = {};
        (function indexNodes(nodes) {
            (nodes || []).forEach(function (n) { idx[n.id] = n; if (n.items && n.items.length) indexNodes(n.items); });
        })(treeData);

        var groupIds = collectAllGroupIds(treeData);
        var requests = groupIds.map(function (gid) {
            var paramName = cfg.groupParamName || 'groupCode';
            var urlA = cfg.apiUrl + '?' + paramName + '=' + encodeURIComponent(gid);
            return $http.get(urlA).then(function (respA) {
                var rawA = (respA.data && respA.data.DATA) ? respA.data.DATA : (Array.isArray(respA.data) ? respA.data : []);
                var normalized = rawA.map(function (x) {
                    var code = x[cfg.codeField] || x.CUSTOMER_CODE || x.PARTY_TYPE_CODE || x.SUPPLIER_CODE || x.CHILD_CUSTOMER_CODE || x.CHILD_PARTY_CODE || x.CHILD_SUPPLIER_CODE || x.CODE || x.ID;
                    var name = x[cfg.nameField] || x.CUSTOMER_EDESC || x.PARTY_TYPE_EDESC || x.SUPPLIER_EDESC || x.CHILD_CUSTOMER_EDESC || x.CHILD_PARTY_EDESC || x.CHILD_SUPPLIER_EDESC || x.NAME || x.EDESC || x.TEXT;
                    return { CODE: code, NAME: name, GROUP_SKU_FLAG: x.GROUP_SKU_FLAG };
                }).filter(function (it) { return !!it.CODE; });
                var leaves = normalized.filter(function (it) { return it.GROUP_SKU_FLAG ? it.GROUP_SKU_FLAG === 'I' : true; });
                var parent = idx[gid];
                if (parent) {
                    parent.items = parent.items || [];
                    leaves.forEach(function (it) {
                        var exists = parent.items.some(function (c) { return c.id === it.CODE; });
                        if (!exists) parent.items.push({ id: it.CODE, text: it.NAME, hasChildren: false });
                    });
                    parent.hasChildren = (parent.items && parent.items.length) > 0;
                }
            }).catch(function () {
                var paramNameB = cfg.groupParamName || 'groupCode';
                var urlB = (cfg.individualsApiUrl || cfg.apiUrl) + '?' + paramNameB + '=' + encodeURIComponent(gid);
                return $http.get(urlB).then(function (respB) {
                    var rawB = (respB.data && respB.data.DATA) ? respB.data.DATA : (Array.isArray(respB.data) ? respB.data : []);
                    var normalizedB = rawB.map(function (x) {
                        var code = x[cfg.codeField] || x.CUSTOMER_CODE || x.PARTY_TYPE_CODE || x.SUPPLIER_CODE || x.CHILD_CUSTOMER_CODE || x.CHILD_PARTY_CODE || x.CHILD_SUPPLIER_CODE || x.CODE || x.ID;
                        var name = x[cfg.nameField] || x.CUSTOMER_EDESC || x.PARTY_TYPE_EDESC || x.SUPPLIER_EDESC || x.CHILD_CUSTOMER_EDESC || x.CHILD_PARTY_EDESC || x.CHILD_SUPPLIER_EDESC || x.NAME || x.EDESC || x.TEXT;
                        return { CODE: code, NAME: name };
                    }).filter(function (it) { return !!it.CODE; });
                    var parentB = idx[gid];
                    if (parentB) {
                        parentB.items = parentB.items || [];
                        normalizedB.forEach(function (it) {
                            var existsB = parentB.items.some(function (c) { return c.id === it.CODE; });
                            if (!existsB) parentB.items.push({ id: it.CODE, text: it.NAME, hasChildren: false });
                        });
                        parentB.hasChildren = (parentB.items && parentB.items.length) > 0;
                    }
                });
            });
        });

        return Promise.all(requests);
    }

    function populateAllIndividualsForTree(partyType) {
        var cfg = partyConfigs[partyType];
        if (!cfg) return Promise.resolve();
        
        var fullTreeData;
        if (partyType === 'customer') {
            fullTreeData = $scope.buildTreeData($scope.customerGroups, 'MASTER_CUSTOMER_CODE', 'PRE_CUSTOMER_CODE', 'CUSTOMER_EDESC', 'CUSTOMER_CODE');
        } else if (partyType === 'dealer') {
            fullTreeData = $scope.buildTreeData($scope.dealerGroups, 'MASTER_PARTY_CODE', 'PRE_PARTY_CODE', 'PARTY_TYPE_EDESC', 'PARTY_TYPE_CODE');
        } else if (partyType === 'supplier') {
            fullTreeData = $scope.buildTreeData($scope.supplierGroups, 'MASTER_SUPPLIER_CODE', 'PRE_SUPPLIER_CODE', 'SUPPLIER_EDESC', 'SUPPLIER_CODE');
        }
        
        if (!fullTreeData || !fullTreeData.length) return Promise.resolve();

        var idx = {};
        (function indexNodes(nodes) {
            (nodes || []).forEach(function (n) { idx[n.id] = n; if (n.items && n.items.length) indexNodes(n.items); });
        })(fullTreeData);

        var groupIds = collectAllGroupIds(fullTreeData);
        
        return $http.post(
            '/api/SetupApi/GetIndividualsByMasterCodes',
            { PartyType: partyType, MasterCodes: groupIds },
            { headers: { 'Content-Type': 'application/json' } }
        ).then(function (resp) {
            var data = (resp.data && (resp.data.DATA || resp.data.Data)) ? (resp.data.DATA || resp.data.Data) : (Array.isArray(resp.data) ? resp.data : []);
            
            var individualsByGroup = {};
            (data || []).forEach(function (x) {
                var code = x[cfg.codeField] || x.CUSTOMER_CODE || x.MASTER_CUSTOMER_CODE || x.MASTER_PARTY_CODE || x.MASTER_SUPPLIER_CODE || x.PARTY_TYPE_CODE || x.SUPPLIER_CODE || x.CHILD_CUSTOMER_CODE || x.CHILD_PARTY_CODE || x.CHILD_SUPPLIER_CODE || x.CODE || x.ID;
                var name = x[cfg.nameField] || x.CUSTOMER_EDESC || x.PARTY_TYPE_EDESC || x.SUPPLIER_EDESC || x.CHILD_CUSTOMER_EDESC || x.CHILD_PARTY_EDESC || x.CHILD_SUPPLIER_EDESC || x.NAME || x.EDESC || x.TEXT;
                var masterCode = x[cfg.masterField] || x.MASTER_CUSTOMER_CODE || x.MASTER_PARTY_CODE || x.MASTER_SUPPLIER_CODE;
                
                if (code && masterCode) {
                    if (!individualsByGroup[masterCode]) {
                        individualsByGroup[masterCode] = [];
                    }
                    individualsByGroup[masterCode].push({ CODE: code, NAME: name });
                }
            });
            
            Object.keys(individualsByGroup).forEach(function (masterCode) {
                var parent = idx[masterCode];
                if (parent) {
                    parent.items = parent.items || [];
                    individualsByGroup[masterCode].forEach(function (it) {
                        var exists = parent.items.some(function (c) { return c.id === it.CODE; });
                        if (!exists) parent.items.push({ id: it.CODE, text: it.NAME, hasChildren: false });
                    });
                    parent.hasChildren = (parent.items && parent.items.length) > 0;
                }
            });
            
            $scope[cfg.selectedTreeDataKey] = fullTreeData;
            
            var totalIndividuals = 0;
            function countIndividuals(nodes) {
                (nodes || []).forEach(function(n) {
                    if (n.items && n.items.length) {
                        countIndividuals(n.items);
                    } else {
                        totalIndividuals++;
                    }
                });
            }
            countIndividuals(fullTreeData);
            
            return fullTreeData;
        }).catch(function (error) {
            var requests = groupIds.map(function (gid) {
                var paramName = cfg.groupParamName || 'groupCode';
                var urlA = cfg.apiUrl + '?' + paramName + '=' + encodeURIComponent(gid);
                
                return $http.get(urlA).then(function (respA) {
                    var rawA = (respA.data && respA.data.DATA) ? respA.data.DATA : (Array.isArray(respA.data) ? respA.data : []);
                    var normalized = rawA.map(function (x) {
                        var code = x[cfg.codeField] || x.CUSTOMER_CODE || x.PARTY_TYPE_CODE || x.SUPPLIER_CODE || x.CHILD_CUSTOMER_CODE || x.CHILD_PARTY_CODE || x.CHILD_SUPPLIER_CODE || x.CODE || x.ID;
                        var name = x[cfg.nameField] || x.CUSTOMER_EDESC || x.PARTY_TYPE_EDESC || x.SUPPLIER_EDESC || x.CHILD_CUSTOMER_EDESC || x.CHILD_PARTY_EDESC || x.CHILD_SUPPLIER_EDESC || x.NAME || x.EDESC || x.TEXT;
                        return { CODE: code, NAME: name, GROUP_SKU_FLAG: x.GROUP_SKU_FLAG };
                    }).filter(function (it) { return !!it.CODE; });
                    var leaves = normalized.filter(function (it) { return it.GROUP_SKU_FLAG ? it.GROUP_SKU_FLAG === 'I' : true; });
                    
                    var parent = idx[gid];
                    if (parent) {
                        parent.items = parent.items || [];
                        leaves.forEach(function (it) {
                            var exists = parent.items.some(function (c) { return c.id === it.CODE; });
                            if (!exists) parent.items.push({ id: it.CODE, text: it.NAME, hasChildren: false });
                        });
                        parent.hasChildren = (parent.items && parent.items.length) > 0;
                    }
                }).catch(function () { });
            });
            
            return Promise.all(requests).then(function() {
                $scope[cfg.selectedTreeDataKey] = fullTreeData;
                
                var totalIndividuals = 0;
                function countIndividuals(nodes) {
                    (nodes || []).forEach(function(n) {
                        if (n.items && n.items.length) {
                            countIndividuals(n.items);
                        } else {
                            totalIndividuals++;
                        }
                    });
                }
                countIndividuals(fullTreeData);
                
                return fullTreeData;
            });
        }).finally(function() { });
    }

    $scope.$watch('rateSchedule.partyType', function (nv, ov) {
        if (!nv || nv === ov) return;
        $scope.currentPartyType = nv;
    });

    var partyConfigs = {
        customer: {
            modalId: '#customerModal',
            treeViewId: '#customerTreeView',
            selectedGridId: '#customerSelectedGrid',
            selectedTreeId: '#selectedCustomersTree',
            selectedDataKey: 'customerSelected',
            selectedTreeDataKey: 'selectedCustomerTreeData',
            selectModeKey: 'customerSelectMode',
            apiUrl: '/api/SetupApi/GetCustomersByGroup',
            individualsApiUrl: '/api/SetupApi/GetChildOfCustomerByGroup',
            groupParamName: 'groupCode',
            codeField: 'CUSTOMER_CODE',
            nameField: 'CUSTOMER_EDESC',
            masterField: 'MASTER_CUSTOMER_CODE'
        },
        dealer: {
            modalId: '#dealerModal',
            treeViewId: '#dealerTreeView',
            selectedGridId: '#dealerSelectedGrid',
            selectedTreeId: '#selectedDealersTree',
            selectedDataKey: 'dealerSelected',
            selectedTreeDataKey: 'selectedDealerTreeData',
            selectModeKey: 'dealerSelectMode',
            apiUrl: '/api/SetupApi/GetDealersByGroup',
            individualsApiUrl: '/api/SetupApi/GetChildOfDealerByGroup',
            groupParamName: 'groupCode',
            codeField: 'PARTY_TYPE_CODE',
            nameField: 'PARTY_TYPE_EDESC',
            masterField: 'MASTER_PARTY_CODE'
        },
        supplier: {
            modalId: '#supplierModal',
            treeViewId: '#supplierTreeView',
            selectedGridId: '#supplierSelectedGrid',
            selectedTreeId: '#selectedSuppliersTree',
            selectedDataKey: 'supplierSelected',
            selectedTreeDataKey: 'selectedSupplierTreeData',
            selectModeKey: 'supplierSelectMode',
            apiUrl: '/api/SetupApi/GetSuppliersByGroup',
            individualsApiUrl: '/api/SetupApi/GetChildOfsupplierByGroup',
            groupParamName: 'groupCode',
            codeField: 'SUPPLIER_CODE',
            nameField: 'SUPPLIER_EDESC',
            masterField: 'MASTER_SUPPLIER_CODE'
        }
    };

    $scope.currentPartyType = 'customer';

    

    $scope.getModalTitle = function () {
        var titles = {
            customer: 'Select Customers',
            dealer: 'Select Dealers',
            supplier: 'Select Suppliers'
        };
        return titles[$scope.currentPartyType] || 'Select Party';
    };

    $scope.getSelectModeLabel = function () {
        var labels = {
            customer: 'Customer Select Mode',
            dealer: 'Dealer Select Mode',
            supplier: 'Supplier Select Mode'
        };
        return labels[$scope.currentPartyType] || 'Select Mode';
    };

    $scope.getCurrentSelectMode = function () {
        var config = partyConfigs[$scope.currentPartyType];
        return $scope[config.selectModeKey];
    };

    $scope.setCurrentSelectMode = function (mode) {
        var config = partyConfigs[$scope.currentPartyType];
        $scope[config.selectModeKey] = mode;
        if ($scope.modalState && $scope.modalState[$scope.currentPartyType]) {
            $scope.modalState[$scope.currentPartyType].selectMode = mode;
        }
        if (mode === 'individual') {
            $scope.refreshGenericSelectedGrid();
        }
    };

    function openPartyModal(partyType) {
        $scope.currentPartyType = partyType;
        var config = partyConfigs[partyType];
        
        var treeId = config.treeViewId;
        var selectedGridId = config.selectedGridId;

        if ($scope.modalState && $scope.modalState[partyType] && $scope.modalState[partyType].selectMode) {
            $scope[config.selectModeKey] = $scope.modalState[partyType].selectMode;
        }

        if (partyType === 'customer') {
            if (!$scope.customerGroups || $scope.customerGroups.length === 0) {
                $scope.loadCustomerGroups();
            }
        } else if (partyType === 'dealer') {
            if (!$scope.dealerGroups || $scope.dealerGroups.length === 0) {
                $scope.loadDealerGroups();
            }
        } else if (partyType === 'supplier') {
            if (!$scope.supplierGroups || $scope.supplierGroups.length === 0) {
                $scope.loadSupplierGroups();
            }
        }

        var selectedData = $scope[config.selectedDataKey] || [];
        for (var i = 0; i < selectedData.length; i++) {
            if (selectedData[i]) selectedData[i].selected = false;
        }

        $(config.modalId).modal('show');

        $timeout(function () {
            updateGridWithData([]);
        }, 100);

        $timeout(function () {
            var searchId = '#' + partyType + 'GridSearch';
            $(searchId).off('input.partyGridSearch').on('input.partyGridSearch', function () {
                var g = $(config.selectedGridId).data('kendoGrid');
                if (!g) return;
                var q = (this.value || '').toString().trim();
                if (!q) {
                    g.dataSource.filter([]);
                    return;
                }
                g.dataSource.filter({
                    logic: 'or',
                    filters: [
                        { field: 'CODE', operator: 'contains', value: q },
                        { field: 'NAME', operator: 'contains', value: q }
                    ]
                });
            });
        }, 150);

        initializeGenericTreeView(partyType);
    }

    function initializeGenericTreeView(partyType) {
        var config = partyConfigs[partyType];
        var treeData = [];

        if (partyType === 'customer') {
            treeData = $scope.buildTreeData($scope.customerGroups, 'MASTER_CUSTOMER_CODE', 'PRE_CUSTOMER_CODE', 'CUSTOMER_EDESC', 'CUSTOMER_CODE');
        } else if (partyType === 'dealer') {
            treeData = $scope.buildTreeData($scope.dealerGroups, 'MASTER_PARTY_CODE', 'PRE_PARTY_CODE', 'PARTY_TYPE_EDESC', 'PARTY_TYPE_CODE');
        } else if (partyType === 'supplier') {
            treeData = $scope.buildTreeData($scope.supplierGroups, 'MASTER_SUPPLIER_CODE', 'PRE_SUPPLIER_CODE', 'SUPPLIER_EDESC', 'SUPPLIER_CODE');
        }

        var ds = new kendo.data.HierarchicalDataSource({
            data: treeData,
            schema: { model: { children: 'items' } }
        });

        var tv = $(config.treeViewId).data("kendoTreeView");
        if (tv) {
            tv.destroy();
            $(config.treeViewId).empty();
        }

        $(config.treeViewId).kendoTreeView({
            dataSource: ds,
            dataTextField: "text",
            checkboxes: { checkChildren: true, threeState: false },
            loadOnDemand: false,
            check: function () {
                var tvLocal = $(config.treeViewId).data("kendoTreeView");
                $scope.modalState[partyType].checkedIds = $scope.getCheckedMasterIds(tvLocal);
                if ($scope[config.selectModeKey] === 'individual') { $scope.refreshGenericSelectedGrid(); }
            },
            change: function () {
                var tvLocal = $(config.treeViewId).data("kendoTreeView");
                $scope.modalState[partyType].checkedIds = $scope.getCheckedMasterIds(tvLocal);
                if ($scope[config.selectModeKey] === 'individual') { $scope.refreshGenericSelectedGrid(); }
            }
        });

        var tvRestore = $(config.treeViewId).data("kendoTreeView");
        var toCheck = ($scope.modalState[partyType] && $scope.modalState[partyType].checkedIds) || [];
        if (tvRestore && toCheck.length) {
            var checkSet = {};
            toCheck.forEach(function (id) { checkSet[id] = true; });
            function walk(nodes) {
                for (var i = 0; i < nodes.length; i++) {
                    var n = nodes[i];
                    if (checkSet[n.id]) {
                        var li = tvRestore.findByUid(n.uid);
                        if (li && li.length) {
                            tvRestore.dataItem(li).set('checked', true);
                        }
                    }
                    if (n.items && n.items.length) walk(n.items);
                }
            }
            walk(tvRestore.dataSource.view());

            $timeout(function () {
                var tvLocal = $(config.treeViewId).data("kendoTreeView");
                if (tvLocal) {
                    $scope.modalState[partyType].checkedIds = $scope.getCheckedMasterIds(tvLocal);
                    if ($scope[config.selectModeKey] === 'individual') {
                        $scope.refreshGenericSelectedGrid();
                    }
                }
            }, 0);
        } else {
            if ($scope[config.selectModeKey] === 'individual') {
                $scope.refreshGenericSelectedGrid();
            }
        }
    }

    $scope.openCustomerModal = function () {
        openPartyModal('customer');
    };

    $scope.openDealerModal = function () {
        openPartyModal('dealer');
    };

    $scope.openSupplierModal = function () {
        openPartyModal('supplier');
    };

    $scope.openItemModal = function () {
        $("#itemModal").modal('show');
        $scope.refreshSelectedItemsGrid();
        
        $timeout(function() {
            $("#itemGridSearch").off('input.itemGridSearch').on('input.itemGridSearch', function () {
                var q = (this.value || '').toString().trim();
                $scope.filterItemGrid(q);
            });
        }, 100);
    };

    function selectParty(partyType) {
        var config = partyConfigs[partyType];
        var selectMode = $scope[config.selectModeKey];
        var tv = $(config.treeViewId).data("kendoTreeView");

        if (!tv) {
            $(config.modalId).modal('hide');
            return;
        }

        function locateById(nodes, id) {
            for (var i = 0; i < nodes.length; i++) {
                var n = nodes[i];
                if (n.id === id) return n;
                var kids = (n.items || []);
                if (kids.length) {
                    var found = locateById(kids, id);
                    if (found) return found;
                }
            }
            return null;
        }
        function cloneSubtree(di) {
            return {
                id: di.id,
                text: di.text,
                hasChildren: (di.items && di.items.length) > 0,
                items: (di.items || []).map(cloneSubtree)
            };
        }
        function stripLeafIndividuals(node) {
            if (!node.items || node.items.length === 0) return node;
            var next = [];
            for (var i = 0; i < node.items.length; i++) {
                var c = node.items[i];
                if (c.items && c.items.length) {
                    next.push(stripLeafIndividuals(c));
                }
            }
            node.items = next;
            node.hasChildren = next.length > 0;
            return node;
        }
        function findChildById(node, id) {
            if (!node || !node.items) return null;
            for (var i = 0; i < node.items.length; i++) {
                if (node.items[i].id === id) return node.items[i];
            }
            return null;
        }
        function indexNodesById(roots) {
            var map = {};
            function walk(n) {
                if (!n) return;
                map[n.id] = n;
                (n.items || []).forEach(walk);
            }
            (roots || []).forEach(walk);
            return map;
        }
        function collectGroupIds(roots) {
            var ids = [];
            function walk(n) {
                if (!n) return;
                ids.push(n.id);
                (n.items || []).forEach(walk);
            }
            (roots || []).forEach(walk);
            return ids;
        }
        function pruneEmptyGroups(roots) {
            function pruneNode(node) {
                if (!node) return null;
                var children = node.items || [];
                if (!children.length) return null; 
                var kept = [];
                for (var i = 0; i < children.length; i++) {
                    var c = children[i];
                    if (c.hasChildren === false || (c.items && c.items.length === 0)) {
                        kept.push(c);
                    } else {
                        var pruned = pruneNode(c);
                        if (pruned) kept.push(pruned);
                    }
                }
                if (kept.length === 0) return null;
                node.items = kept;
                node.hasChildren = kept.length > 0;
                return node;
            }
            var result = [];
            for (var r = 0; r < (roots || []).length; r++) {
                var pr = pruneNode(roots[r]);
                if (pr) result.push(pr);
            }
            return result;
        }

        var nodes = $scope.getCheckedNodesInfo(tv);
        var groups = nodes.filter(function (n) { return !n.isLeaf; });
        var leaves = nodes.filter(function (n) { return n.isLeaf; });
        var selectedGroupIds = groups.map(function (g) { return g.id; });
        var selectedGroupSet = {}; selectedGroupIds.forEach(function (id) { selectedGroupSet[id] = true; });

        var modalRoots = tv.dataSource.view();

        var skeleton = [];
        for (var gi = 0; gi < selectedGroupIds.length; gi++) {
            var gid = selectedGroupIds[gi];
            var src = locateById(modalRoots, gid);
            if (src) {
                skeleton.push(cloneSubtree(src));
            }
        }

        if (selectMode === 'individual' && skeleton.length > 0) {
        }
        if (selectMode === 'group' && skeleton.length === 0 && leaves.length > 0) {
            var leafIndividuals = leaves.map(function (n) { return { CODE: n.originalCode || n.id, NAME: n.text }; });
            var grouped = buildGroupedTreeFromIndividuals(config.treeViewId, leafIndividuals);
            $scope[config.selectedDataKey] = leafIndividuals;
            $scope[config.selectedTreeDataKey] = grouped;
            updateTreeAndClose(grouped, config, partyType);
            return;
        }

        function attachIndividualsToSkeleton(roots, individuals, masterField) {
            var idx = indexNodesById(roots);
            var flatSelected = [];

            for (var i = 0; i < individuals.length; i++) {
                var it = individuals[i];
                var gId = it[masterField];
                var code = it[config.codeField];
                var name = it[config.nameField];
                if (!gId || !code) continue;
                var parent = idx[gId];
                if (!parent) continue;
                parent.items = parent.items || [];
                var exists = false;
                for (var j = 0; j < parent.items.length; j++) { if (parent.items[j].id === code) { exists = true; break; } }
                if (!exists) {
                    parent.items.push({ id: code, text: name, hasChildren: false });
                    parent.hasChildren = true;
                }
                flatSelected.push({ CODE: code, NAME: name });
            }
            return flatSelected;
        }

        var allGroupIds = collectGroupIds(skeleton);

        if (selectMode === 'individual') {
            var chosenIndividuals = getSelectedIndividuals(config) || [];
            if (chosenIndividuals.length === 0) {
                alert('Please select at least one ' + partyType + ' in Individual mode, or switch to Group mode.');
                return; 
            }
            if (selectedGroupIds.length === 0) {
                var groupedInd = buildSpecificPathTreeFromIndividuals(config.treeViewId, chosenIndividuals);
                $scope[config.selectedDataKey] = chosenIndividuals.map(function (x) { return { CODE: x.CODE, NAME: x.NAME, selected: true }; });
                $scope[config.selectedTreeDataKey] = groupedInd;
                updateTreeAndClose(groupedInd, config, partyType);
                return;
            }
        }

        if (allGroupIds.length === 0) {
            $scope[config.selectedTreeDataKey] = [];
            updateTreeAndClose([], config, partyType);
            return;
        }

        var chosen = (selectMode === 'individual') ? (getSelectedIndividuals(config) || []) : [];
        var allowed = {};
        for (var c = 0; c < chosen.length; c++) { allowed[chosen[c].CODE] = true; }
        var idx = indexNodesById(skeleton);
        var promises = [];
        var flatIndividuals = [];

        allGroupIds.forEach(function (gid) {
            promises.push(
                (function (g) {
                    var paramNameA = config.groupParamName || 'groupCode';
                    var urlA = config.apiUrl + '?' + paramNameA + '=' + encodeURIComponent(g);
                    return $http.get(urlA)
                        .then(function (respA) {
                            var rawA = (respA.data && respA.data.DATA) ? respA.data.DATA : (Array.isArray(respA.data) ? respA.data : []);
                            var normalizedA = rawA.map(function (x) {
                                var code = x[config.codeField] || x.CUSTOMER_CODE || x.PARTY_TYPE_CODE || x.SUPPLIER_CODE || x.CHILD_CUSTOMER_CODE || x.CHILD_PARTY_CODE || x.CHILD_SUPPLIER_CODE || x.CODE || x.ID;
                                var name = x[config.nameField] || x.CUSTOMER_EDESC || x.PARTY_TYPE_EDESC || x.SUPPLIER_EDESC || x.CHILD_CUSTOMER_EDESC || x.CHILD_PARTY_EDESC || x.CHILD_SUPPLIER_EDESC || x.NAME || x.EDESC || x.TEXT;
                                return { CODE: code, NAME: name, GROUP_SKU_FLAG: x.GROUP_SKU_FLAG };
                            }).filter(function (it) { return !!it.CODE; });
                            var onlyA = normalizedA.filter(function (it) {
                                if (it.GROUP_SKU_FLAG) return it.GROUP_SKU_FLAG === 'I';
                                return isIndividual(it.CODE, partyType);
                            });
                            if (selectMode === 'individual' && Object.keys(allowed).length > 0) {
                                onlyA = onlyA.filter(function (it) { return !!allowed[it.CODE]; });
                            }
                            if (onlyA.length > 0) {
                                var parentA = idx[g];
                                if (parentA) {
                                    parentA.items = parentA.items || [];
                                    onlyA.forEach(function (it) {
                                        var existsA = false;
                                        for (var jA = 0; jA < parentA.items.length; jA++) { if (parentA.items[jA].id === it.CODE) { existsA = true; break; } }
                                        if (!existsA) parentA.items.push({ id: it.CODE, text: it.NAME, hasChildren: false });
                                    });
                                    parentA.hasChildren = (parentA.items && parentA.items.length) > 0;
                                }
                                onlyA.forEach(function (it) { flatIndividuals.push({ CODE: it.CODE, NAME: it.NAME }); });
                                return; 
                            }
                            var paramNameB = config.groupParamName || 'groupCode';
                            var urlB = (config.individualsApiUrl || config.apiUrl) + '?' + paramNameB + '=' + encodeURIComponent(g);
                            return $http.get(urlB).then(function (respB) {
                                var rawB = (respB.data && respB.data.DATA) ? respB.data.DATA : (Array.isArray(respB.data) ? respB.data : []);
                                var normalizedB = rawB.map(function (x) {
                                    var code = x[config.codeField] || x.CUSTOMER_CODE || x.PARTY_TYPE_CODE || x.SUPPLIER_CODE || x.CHILD_CUSTOMER_CODE || x.CHILD_PARTY_CODE || x.CHILD_SUPPLIER_CODE || x.CODE || x.ID;
                                    var name = x[config.nameField] || x.CUSTOMER_EDESC || x.PARTY_TYPE_EDESC || x.SUPPLIER_EDESC || x.CHILD_CUSTOMER_EDESC || x.CHILD_PARTY_EDESC || x.CHILD_SUPPLIER_EDESC || x.NAME || x.EDESC || x.TEXT;
                                    return { CODE: code, NAME: name, GROUP_SKU_FLAG: x.GROUP_SKU_FLAG };
                                }).filter(function (it) { return !!it.CODE; });
                                var onlyB = normalizedB.filter(function (it) {
                                    if (it.GROUP_SKU_FLAG) return it.GROUP_SKU_FLAG === 'I';
                                    return isIndividual(it.CODE, partyType);
                                });
                                if (selectMode === 'individual' && Object.keys(allowed).length > 0) {
                                    onlyB = onlyB.filter(function (it) { return !!allowed[it.CODE]; });
                                }
                                var parentB = idx[g];
                                if (parentB) {
                                    parentB.items = parentB.items || [];
                                    onlyB.forEach(function (it) {
                                        var existsB = false;
                                        for (var jB = 0; jB < parentB.items.length; jB++) { if (parentB.items[jB].id === it.CODE) { existsB = true; break; } }
                                        if (!existsB) parentB.items.push({ id: it.CODE, text: it.NAME, hasChildren: false });
                                    });
                                    parentB.hasChildren = (parentB.items && parentB.items.length) > 0;
                                }
                                onlyB.forEach(function (it) { flatIndividuals.push({ CODE: it.CODE, NAME: it.NAME }); });
                            });
                        })
                        .catch(function (err) {
                            var paramNameB2 = config.groupParamName || 'groupCode';
                            var urlB2 = (config.individualsApiUrl || config.apiUrl) + '?' + paramNameB2 + '=' + encodeURIComponent(g);
                            return $http.get(urlB2).then(function (respB2) {
                                var rawB2 = (respB2.data && respB2.data.DATA) ? respB2.data.DATA : (Array.isArray(respB2.data) ? respB2.data : []);
                                var normalizedB2 = rawB2.map(function (x) {
                                    var code = x[config.codeField] || x.CUSTOMER_CODE || x.PARTY_TYPE_CODE || x.SUPPLIER_CODE || x.CHILD_CUSTOMER_CODE || x.CHILD_PARTY_CODE || x.MASTER_CUSTOMER_CODE || x.MASTER_PARTY_CODE || x.MASTER_SUPPLIER_CODE || x.CHILD_SUPPLIER_CODE || x.CODE || x.ID;
                                    var name = x[config.nameField] || x.CUSTOMER_EDESC || x.PARTY_TYPE_EDESC || x.SUPPLIER_EDESC || x.CHILD_CUSTOMER_EDESC || x.CHILD_PARTY_EDESC || x.CHILD_SUPPLIER_EDESC || x.NAME || x.EDESC || x.TEXT;
                                    return { CODE: code, NAME: name, GROUP_SKU_FLAG: x.GROUP_SKU_FLAG };
                                }).filter(function (it) { return !!it.CODE; });
                                var onlyB2 = normalizedB2.filter(function (it) {
                                    if (it.GROUP_SKU_FLAG) return it.GROUP_SKU_FLAG === 'I';
                                    return isIndividual(it.CODE, partyType);
                                });
                                if (selectMode === 'individual' && Object.keys(allowed).length > 0) {
                                    onlyB2 = onlyB2.filter(function (it) { return !!allowed[it.CODE]; });
                                }
                                var parentB2 = idx[g];
                                if (parentB2) {
                                    parentB2.items = parentB2.items || [];
                                    onlyB2.forEach(function (it) {
                                        var existsB2 = false;
                                        for (var jB2 = 0; jB2 < parentB2.items.length; jB2++) { if (parentB2.items[jB2].id === it.CODE) { existsB2 = true; break; } }
                                        if (!existsB2) parentB2.items.push({ id: it.CODE, text: it.NAME, hasChildren: false });
                                    });
                                    parentB2.hasChildren = (parentB2.items && parentB2.items.length) > 0;
                                }
                                onlyB2.forEach(function (it) { flatIndividuals.push({ CODE: it.CODE, NAME: it.NAME }); });
                            });
                        });
                })(gid)
            );
        });

        Promise.all(promises).then(function () {
            $scope[config.selectedDataKey] = (selectMode === 'individual')
                ? flatIndividuals.map(function (it) { return { CODE: it.CODE, NAME: it.NAME, selected: true }; })
                : flatIndividuals;
            var finalTree = (selectMode === 'individual') ? pruneEmptyGroups(skeleton) : skeleton;
            $scope[config.selectedTreeDataKey] = finalTree;
            updateTreeAndClose(finalTree, config, partyType);
        });
    }

    function isIndividual(code, partyType) {
        if (partyType === 'customer') {
            return $scope.isCustomerIndividual(code);
        } else if (partyType === 'dealer') {
            return $scope.isDealerIndividual(code);
        } else if (partyType === 'supplier') {
            return $scope.isSupplierIndividual(code);
        }
        return true;
    }

    function getSelectedIndividuals(config) {
        var grid = $(config.selectedGridId).data("kendoGrid");
        var selected = [];

        if (grid) {
            var view = grid.dataSource.view();
            for (var i = 0; i < view.length; i++) {
                if (view[i].selected) {
                    selected.push({
                        CODE: view[i].CODE,
                        NAME: view[i].NAME
                    });
                }
            }
        }

        return selected;
    }

    function updateTreeAndClose(finalNodes, config, partyType) {
        $scope[config.selectedTreeDataKey] = finalNodes;
        buildSelectedTree(partyType);
        function collectLeafIndividuals(nodes, acc) {
            (nodes || []).forEach(function (n) {
                var hasKids = n.items && n.items.length;
                if (!hasKids) {
                    acc.push({ CODE: n.id, NAME: n.text, selected: true });
                } else {
                    collectLeafIndividuals(n.items, acc);
                }
            });
        }

        var leaves = [];
        collectLeafIndividuals(finalNodes || [], leaves);
        if (partyType === 'customer') {
            $scope.customerSelected = leaves;
            if (typeof $scope.refreshCustomerSelectedGrid === 'function') { $scope.refreshCustomerSelectedGrid(); }
        } else if (partyType === 'dealer') {
            $scope.dealerSelected = leaves;
            var dg = $(config.selectedGridId).data('kendoGrid');
            if (dg) { dg.dataSource.data(leaves); dg.refresh(); }
        } else if (partyType === 'supplier') {
            $scope.supplierSelected = leaves;
            var sg = $(config.selectedGridId).data('kendoGrid');
            if (sg) { sg.dataSource.data(leaves); sg.refresh(); }
        }
        $(config.modalId).modal('hide');
        updateSummaries();
        $scope.$applyAsync();
    }

    function buildSelectedTree(partyType) {
        var config = partyConfigs[partyType];
        var treeData = $scope[config.selectedTreeDataKey] || [];

        var ds = new kendo.data.HierarchicalDataSource({
            data: treeData,
            schema: { model: { children: 'items' } }
        });

        var tv = $(config.selectedTreeId).data("kendoTreeView");
        if (tv) {
            tv.destroy();
            $(config.selectedTreeId).empty();
        }

        $(config.selectedTreeId).kendoTreeView({
            dataSource: ds,
            dataTextField: "text",
            checkboxes: { checkChildren: true, threeState: false },
            loadOnDemand: false,
            dataBound: function (e) {
                var treeView = e.sender;
                if (!treeView._hasExpanded) {
                    treeView._hasExpanded = true;
                    setTimeout(function () {
                        try {
                            treeView.expand('.k-item');
                        } catch (expandError) {
                        }
                    }, 200);
                }
            }
        });
    }

    $scope.applyRateGridSearch = function () {
        var q = ($scope.rateGridSearch || '').toString().trim();
        var grid = $("#rateGrid").data("kendoGrid");
        if (!grid) return;
        if (!q) {
            grid.dataSource.filter([]);
            return;
        }
        grid.dataSource.filter({
            logic: 'or',
            filters: [
                { field: 'ITEM_CODE', operator: 'contains', value: q },
                { field: 'ITEM_EDESC', operator: 'contains', value: q }
            ]
        });
    };
    function filterTreeDataByText(nodes, textLower) {
        var res = [];
        (nodes || []).forEach(function (n) {
            var children = filterTreeDataByText(n.items || [], textLower);
            var selfMatch = (n.text || '').toString().toLowerCase().indexOf(textLower) > -1;
            if (selfMatch || children.length) {
                res.push({ id: n.id, text: n.text, hasChildren: (children.length > 0), items: children });
            }
        });
        return res;
    }

    $scope.applySelectedTreeSearch = function () {
        var txt = ($scope.selectedTreeSearch || '').toString().trim();
        var cfg = partyConfigs[$scope.rateSchedule.partyType];
        if (!cfg) return;
        var base = $scope[cfg.selectedTreeDataKey] || [];
        var tv = $(cfg.selectedTreeId).data('kendoTreeView');

        if (!txt) {
            if (tv) {
                var dsFull = new kendo.data.HierarchicalDataSource({ data: base, schema: { model: { children: 'items' } } });
                tv.setDataSource(dsFull);
                setTimeout(function () { try { tv.expand('.k-item'); } catch (e) {} }, 0);
            } else {
                try { buildSelectedTree($scope.rateSchedule.partyType); } catch (e2) {}
            }
            return;
        }

        var filtered = filterTreeDataByText(base, txt.toLowerCase());
        if (tv) {
            var ds = new kendo.data.HierarchicalDataSource({ data: filtered, schema: { model: { children: 'items' } } });
            tv.setDataSource(ds);
            setTimeout(function () { try { tv.expand('.k-item'); } catch (e3) {} }, 0);
        } else {
            try {
                $(cfg.selectedTreeId).kendoTreeView({
                    dataSource: new kendo.data.HierarchicalDataSource({ data: filtered, schema: { model: { children: 'items' } } }),
                    dataTextField: 'text',
                    checkboxes: { checkChildren: true, threeState: false },
                    loadOnDemand: false
                });
            } catch (e4) { }
        }
    };

    $scope.selectAllSelectedTree = function () {
        var cfg = partyConfigs[$scope.rateSchedule.partyType];
        if (!cfg) return;
        var tv = $(cfg.selectedTreeId).data('kendoTreeView');
        if (!tv) return;
        function walk(nodes) {
            (nodes || []).forEach(function (n) {
                var li = tv.findByUid(n.uid);
                if (li && li.length) {
                    tv.dataItem(li).set('checked', true);
                }
                if (n.items && n.items.length) walk(n.items);
            });
        }
        walk(tv.dataSource.view());
        function collectLeaves(nodes, acc) {
            (nodes || []).forEach(function (n) {
                if (n.items && n.items.length) {
                    collectLeaves(n.items, acc);
                } else {
                    acc.push({ CODE: n.id, NAME: n.text, selected: true });
                }
            });
        }
        var leaves = [];
        collectLeaves($scope[cfg.selectedTreeDataKey] || [], leaves);
        if ($scope.rateSchedule.partyType === 'customer') {
            $scope.customerSelected = leaves;
        } else if ($scope.rateSchedule.partyType === 'dealer') {
            $scope.dealerSelected = leaves;
        } else if ($scope.rateSchedule.partyType === 'supplier') {
            $scope.supplierSelected = leaves;
        }
        if (typeof updateSummaries === 'function') { try { updateSummaries(); } catch (e) {} }
        $scope.$applyAsync();
    };

    $scope.clearAllSelectedTree = function () {
        var cfg = partyConfigs[$scope.rateSchedule.partyType];
        if (!cfg) return;
        var tv = $(cfg.selectedTreeId).data('kendoTreeView');
        if (!tv) return;
        function walk(nodes) {
            (nodes || []).forEach(function (n) {
                var li = tv.findByUid(n.uid);
                if (li && li.length) {
                    tv.dataItem(li).set('checked', false);
                }
                if (n.items && n.items.length) walk(n.items);
            });
        }
        walk(tv.dataSource.view());
        if ($scope.rateSchedule.partyType === 'customer') {
            $scope.customerSelected = [];
        } else if ($scope.rateSchedule.partyType === 'dealer') {
            $scope.dealerSelected = [];
        } else if ($scope.rateSchedule.partyType === 'supplier') {
            $scope.supplierSelected = [];
        }
        if (typeof updateSummaries === 'function') { try { updateSummaries(); } catch (e2) {} }
        $scope.$applyAsync();
    };

    $scope.refreshGenericSelectedGrid = function () {
        debugger
        var config = partyConfigs[$scope.currentPartyType];
        var tv = $(config.treeViewId).data("kendoTreeView");

        if (!tv) {
            updateGridWithData([]);
            return;
        }

        var nodes = $scope.getCheckedNodesInfo(tv);
        var groups = nodes.filter(function (n) { return !n.isLeaf; });
        var leaves = nodes.filter(function (n) { return n.isLeaf; });

        if (groups.length > 0) {
            var masterCodes = groups.map(function (g) { return g.id; });
            var allIndividuals = [];

            $http.post(
                '/api/SetupApi/GetIndividualsByMasterCodes',
                { PartyType: $scope.currentPartyType, MasterCodes: masterCodes },
                { headers: { 'Content-Type': 'application/json' } }
            )
                .then(function (resp) {
                    var data = (resp.data && resp.data.DATA) ? resp.data.DATA : [];
                    var normalized = data.map(function (x) {
                        return { CODE: x.CODE || x.CUSTOMER_CODE || x.PARTY_CODE || x.SUPPLIER_CODE || x.MASTER_CUSTOMER_CODE || x.MASTER_PARTY_CODE || x.MASTER_SUPPLIER_CODE || x.ITEM_CODE || x.ID, NAME: x.NAME || x.CUSTOMER_EDESC || x.PARTY_EDESC || x.SUPPLIER_EDESC || x.ITEM_EDESC || x.EDESC || x.TEXT };
                    }).filter(function (x) { return !!x.CODE; });
                    allIndividuals = normalized;
                    updateGridWithData(allIndividuals);
                    var unique = [];
                    var seen = {};
                    allIndividuals.forEach(function (ind) { if (!seen[ind.CODE]) { seen[ind.CODE] = true; unique.push(ind); } });
                    updateGridWithData(unique);
                });
        } else if (leaves.length > 0) {
            var leafIndividuals = leaves.map(function (n) {
                return {
                    CODE: n.originalCode || n.id,
                    NAME: n.text,
                    selected: false
                };
            });
            updateGridWithData(leafIndividuals);
        } else {
            updateGridWithData([]);
        }
    };

    function updateGridWithData(individuals) {
        var config = partyConfigs[$scope.currentPartyType];
        var grid = $(config.selectedGridId).data("kendoGrid");

        var state = $scope.modalState[$scope.currentPartyType] || { gridSelectedCodes: [] };
        var selectedSet = {};
        (state.gridSelectedCodes || []).forEach(function (c) { selectedSet[c] = true; });
        var prepared = (individuals || []).map(function (x) { return { CODE: x.CODE, NAME: x.NAME, selected: !!selectedSet[x.CODE] }; });

        var gridOptions = {
            dataSource: {
                data: prepared,
                schema: { model: { id: 'CODE', fields: { CODE: { type: 'string' }, NAME: { type: 'string' }, selected: { type: 'boolean', defaultValue: false } } } }
            },
            selectable: false,
            columns: [
                { headerTemplate: "<input type='checkbox' class='party-header-check' />", width: 44, attributes: { style: 'text-align:center;' }, headerAttributes: { style: 'text-align:center; width:44px;' }, template: "<input type='checkbox' class='party-row-check' #= selected ? 'checked=checked' : '' # />" },
                { field: 'CODE', title: 'Code', width: 120 },
                { field: 'NAME', title: 'Description' }
            ],
            dataBound: function () {
                var g = this;
                var $grid = $(g.element);

                function updateHeaderState() {
                    var view = g.dataSource.view();
                    if (!view || view.length === 0) { $grid.find('.party-header-check').prop({ checked: false, indeterminate: false }); return; }
                    var total = view.length;
                    var selectedCount = 0;
                    for (var i = 0; i < total; i++) { if (view[i].selected) selectedCount++; }
                    var all = selectedCount === total;
                    var none = selectedCount === 0;
                    var ind = !all && !none;
                    var $hdr = $grid.find('.party-header-check');
                    $hdr.prop('checked', all);
                    $hdr.prop('indeterminate', ind);
                }

                g.tbody.off('change.party').on('change.party', '.party-row-check', function () {
                    var tr = $(this).closest('tr');
                    var di = g.dataItem(tr);
                    if (!di) return;
                    di.set('selected', this.checked);
                    var view = g.dataSource.view();
                    var codes = [];
                    for (var i = 0; i < view.length; i++) { if (view[i].selected) codes.push(view[i].CODE); }
                    $scope.modalState[$scope.currentPartyType].gridSelectedCodes = codes;
                    updateHeaderState();
                });

                $grid.find('.party-header-check').off('change.partyAll').on('change.partyAll', function () {
                    var check = this.checked;
                    var view = g.dataSource.view();
                    for (var i = 0; i < view.length; i++) { view[i].set('selected', check); }
                    var codes = [];
                    if (check) { for (var j = 0; j < view.length; j++) { codes.push(view[j].CODE); } }
                    $scope.modalState[$scope.currentPartyType].gridSelectedCodes = codes;
                    g.tbody.find('.party-row-check').prop('checked', check);
                    updateHeaderState();
                });

                updateHeaderState();
            }
        };

        if (!grid) {
            $(config.selectedGridId).kendoGrid(gridOptions);
        } else {
            grid.setOptions(gridOptions);
            // Use the prepared dataset so each item has a 'selected' field expected by the template
            grid.dataSource.data(prepared);
            grid.refresh();
        }
    }

    $scope.moveCurrentPartyToGrid = function () {
        debugger
        var config = partyConfigs[$scope.currentPartyType];
        var tv = $(config.treeViewId).data("kendoTreeView");
        if (!tv) return;

        var nodes = $scope.getCheckedNodesInfo(tv);
        var groups = nodes.filter(function (n) { return !n.isLeaf; });
        var leaves = nodes.filter(function (n) { return n.isLeaf; });

        if (groups.length === 0 && leaves.length === 0) return;

        var masterCodes = groups.map(function (g) { return g.id; });
        var apiEndpoint = '/api/SetupApi/GetIndividualsByMasterCodes';

        if (masterCodes.length > 0) {
            $http.post(apiEndpoint, { PartyType: $scope.currentPartyType, MasterCodes: masterCodes })
                .then(function (resp) {
 
                    var individuals = (resp.data && resp.data.DATA) ? resp.data.DATA : [];
                    var formattedIndividuals = individuals.map(function (x) {
                        var code = x[config.codeField] || x.CUSTOMER_CODE || x.PARTY_TYPE_CODE || x.SUPPLIER_CODE || x.CHILD_CUSTOMER_CODE || x.CHILD_PARTY_CODE || x.CHILD_SUPPLIER_CODE || x.CODE || x.ID;
                        var name = x[config.nameField] || x.CUSTOMER_EDESC || x.PARTY_TYPE_EDESC || x.SUPPLIER_EDESC || x.CHILD_CUSTOMER_EDESC || x.CHILD_PARTY_EDESC || x.CHILD_SUPPLIER_EDESC || x.NAME || x.EDESC || x.TEXT;
                        return { CODE: code, NAME: name, selected: false };
                    });
 
                    var existingData = $scope[config.selectedDataKey] || [];
                    var codeMap = {};
                    existingData.forEach(function (item) { codeMap[item.CODE] = true; });
                    formattedIndividuals.forEach(function (item) { if (!codeMap[item.CODE]) { existingData.push(item); } });
                    $scope[config.selectedDataKey] = existingData;
                    var grid = $(config.selectedGridId).data("kendoGrid");
                    if (grid) { grid.dataSource.data(existingData); }
                    $scope.$applyAsync();
                })
                .catch(function (error) {
 
                });
        } else {
            var leafIndividuals = leaves.filter(function (n) {
                return isIndividual(n.originalCode || n.id, $scope.currentPartyType);
            }).map(function (n) {
                return {
                    CODE: n.originalCode || n.id,
                    NAME: n.text,
                    selected: false
                };
            });

            var existingData = $scope[config.selectedDataKey] || [];
            var codeMap = {};
            existingData.forEach(function (item) { codeMap[item.CODE] = true; });
            leafIndividuals.forEach(function (item) { if (!codeMap[item.CODE]) { existingData.push(item); } });
            $scope[config.selectedDataKey] = existingData;
            var grid = $(config.selectedGridId).data("kendoGrid");
            if (grid) { grid.dataSource.data(existingData); }
        }
    };

    $scope.removeCurrentPartyFromGrid = function () {
        var config = partyConfigs[$scope.currentPartyType];
        var grid = $(config.selectedGridId).data("kendoGrid");
        if (!grid) return;

        var view = grid.dataSource.view();
        var selectedCodes = [];

        for (var i = 0; i < view.length; i++) {
            if (view[i].selected) {
                selectedCodes.push(view[i].CODE);
            }
        }

        if (selectedCodes.length === 0) return;

        var existingData = $scope[config.selectedDataKey] || [];
        $scope[config.selectedDataKey] = existingData.filter(function (item) {
            return selectedCodes.indexOf(item.CODE) === -1;
        });
    };

    $scope.selectCurrentParty = function () {
        selectParty($scope.currentPartyType);
    };

    $scope.selectCustomer = function () {
        selectParty('customer');
    };

    $scope.selectDealer = function () {
        selectParty('dealer');
    };

    $scope.selectSupplier = function () {
        selectParty('supplier');
    };

    $scope.buildSelectedCustomersTree = function () {
        buildSelectedTree('customer');
    };

    $scope.buildSelectedDealersTree = function () {
        buildSelectedTree('dealer');
    };

    $scope.buildSelectedSuppliersTree = function () {
        buildSelectedTree('supplier');
    };

    $scope.init = function () {
        $scope.loadDefaultCurrency();
        $scope.loadCurrencyList();
        $scope.loadAreaList();
        $scope.loadDocumentList();
        $scope.loadCustomerGroups();
        $scope.loadDealerGroups();
        $scope.loadSupplierGroups();
        $scope.loadItemGroups();
        $scope.initializeGrids();
    };

    $scope.loadDefaultCurrency = function () {
        $http.get('/api/SetupApi/GetDefaultCurrency')
            .then(function (response) {
                if (response.data.STATUS_CODE === 200 && response.data.DATA) {
                    $scope.rateSchedule.currencyCode = response.data.DATA.CURRENCY_CODE;
                    $scope.rateSchedule.exchangeRate = response.data.DATA.EXCHANGE_RATE;
                }
            })
            .catch(function (error) {
 
            });
    };

    $scope.loadCurrencyList = function () {
        $http.get('/api/SetupApi/GetCurrencyListForRateSchedule')
            .then(function (response) {
                if (response.data.STATUS_CODE === 200) {
                    $scope.currencyList = response.data.DATA || [];
                }
            })
            .catch(function (error) {
 
            });
    };

    $scope.loadAreaList = function () {
        $http.get('/api/SetupApi/GetAreaList')
            .then(function (response) {
                if (response.data.STATUS_CODE === 200) {
                    $scope.areaList = response.data.DATA || [];
                }
            })
            .catch(function (error) {
 
            });
    };

    $scope.loadDocumentList = function () {
        $http.get('/api/SetupApi/GetDocumentList')
            .then(function (response) {
                if (response.data.STATUS_CODE === 200) {
                    $scope.documentList = response.data.DATA || [];
                }
            })
            .catch(function (error) {
 
            });
    };

    $scope.loadCustomerGroups = function () {
        $http.get('/api/SetupApi/GetCustomerGroups')
            .then(function (response) {
                if (response.data && response.data.STATUS_CODE === 200) {
                    $scope.customerGroups = response.data.DATA || [];
                    $scope.buildCustomerHierarchySets();
                    $scope.initializeCustomerTreeView();
                } else {
                    $scope.customerGroups = [];
                    $scope.buildCustomerHierarchySets();
                    $scope.initializeCustomerTreeView();
                }
            })
            .catch(function (error) {
 
                $scope.customerGroups = [];
                $scope.buildCustomerHierarchySets();
                $scope.initializeCustomerTreeView();
            });
    };

    $scope.loadDealerGroups = function () {
        $http.get('/api/SetupApi/GetDealerGroups')
            .then(function (response) {
                if (response.data && response.data.STATUS_CODE === 200) {
                    $scope.dealerGroups = response.data.DATA || [];
                    $scope.initializeDealerTreeView();
                } else {
                    $scope.dealerGroups = [];
                    $scope.initializeDealerTreeView();
                }
            })
            .catch(function (error) {
 
                $scope.dealerGroups = [];
                $scope.initializeDealerTreeView();
            });
    };

    $scope.loadSupplierGroups = function () {
        $http.get('/api/SetupApi/GetSupplierGroups')
            .then(function (response) {
                if (response.data && response.data.STATUS_CODE === 200) {
                    $scope.supplierGroups = response.data.DATA || [];
                    $scope.initializeSupplierTreeView();
                } else {
                    $scope.supplierGroups = [];
                    $scope.initializeSupplierTreeView();
                }
            })
            .catch(function (error) {
 
                $scope.supplierGroups = [];
                $scope.initializeSupplierTreeView();
            });
    };

    $scope.loadItemGroups = function () {
        $http.get('/api/SetupApi/GetItemGroups')
            .then(function (response) {
                if (response.data && response.data.STATUS_CODE === 200) {
                    $scope.itemGroups = response.data.DATA || [];
                    $scope.initializeItemTreeView();
                } else {
                    $scope.itemGroups = [];
                    $scope.initializeItemTreeView();
                }
            })
            .catch(function (error) {
 
                $scope.itemGroups = [];
                $scope.initializeItemTreeView();
            });
    };

    $scope.initializeGrids = function () {
        $("#rateGrid").kendoGrid({
            dataSource: {
                data: $scope.rateSchedule.rateData,
                schema: {
                    model: {
                        id: "ITEM_CODE",
                        fields: {
                            ITEM_CODE: { type: "string" },
                            ITEM_EDESC: { type: "string" },
                            MU_CODE: { type: "string" },
                            STANDARD_RATE: { type: "number" },
                            MRP_RATE: { type: "number" },
                            RETAIL_PRICE: { type: "number" }
                        }
                    }
                }
            },
            height: 400,
            scrollable: true,
            sortable: true,
            filterable: true,
            pageable: {
                refresh: true,
                pageSizes: true,
                buttonCount: 5
            },
            columns: [
                { field: "ITEM_CODE", title: "Item Code", width: "120px" },
                { field: "ITEM_EDESC", title: "Item Description", width: "200px" },
                {
                    field: "MU_CODE", title: "Unit", width: "100px",
                    editor: function (container, options) {
                        $('<input data-bind="value:' + options.field + '" class="k-textbox" type="text" />')
                            .appendTo(container);
                    }
                },
                {
                    field: "STANDARD_RATE",
                    title: "Standard Rate",
                    width: "120px",
                    format: "{0:n2}",
                    editor: function (container, options) {
                        $('<input data-bind="value:' + options.field + '" class="k-textbox" type="number" step="0.01" />')
                            .appendTo(container)
                    }
                },
                {
                    field: "MRP_RATE",
                    title: "Premium / MRP Rate",
                    width: "120px",
                    format: "{0:n2}",
                    editor: function (container, options) {
                        $('<input data-bind="value:' + options.field + '" class="k-textbox" type="number" step="0.01" />')
                            .appendTo(container)
                    }
                },
                {
                    field: "RETAIL_PRICE",
                    title: "Retail Price",
                    width: "120px",
                    format: "{0:n2}"
                }
            ],
            editable: "incell"
        });

    };

    $scope.initializeCustomerTreeView = function () {
        var customerTreeData = $scope.buildTreeData($scope.customerGroups, 'MASTER_CUSTOMER_CODE', 'PRE_CUSTOMER_CODE', 'CUSTOMER_EDESC', 'CUSTOMER_CODE');
        var ds = new kendo.data.HierarchicalDataSource({
            data: customerTreeData,
            schema: { model: { children: 'items' } }
        });
        var tv = $("#customerTreeView").data("kendoTreeView");
        if (tv) { tv.destroy(); $("#customerTreeView").empty(); }
        $("#customerTreeView").kendoTreeView({
            dataSource: ds,
            dataTextField: "text",
            checkboxes: { checkChildren: true, threeState: false },
            loadOnDemand: false,
            select: function (e) {
                var dataItem = this.dataItem(e.node);
                if (dataItem.hasChildren) {
                    $scope.expandCustomerGroup(dataItem.id);
                }
            }
        });
    };

    $scope.initializeDealerTreeView = function () {
        var dealerTreeData = $scope.buildTreeData($scope.dealerGroups, 'MASTER_PARTY_CODE', 'PRE_PARTY_CODE', 'PARTY_TYPE_EDESC', 'PARTY_TYPE_CODE');
        var ds = new kendo.data.HierarchicalDataSource({
            data: dealerTreeData,
            schema: { model: { children: 'items' } }
        });
        var tv = $("#dealerTreeView").data("kendoTreeView");
        if (tv) { tv.destroy(); $("#dealerTreeView").empty(); }
        $("#dealerTreeView").kendoTreeView({
            dataSource: ds,
            dataTextField: "text",
            checkboxes: { checkChildren: true, threeState: false },
            loadOnDemand: false,
            change: function () {
                if ($scope.currentPartyType === 'dealer') {
                    $scope.refreshGenericSelectedGrid();
                }
            }
        });
    };

    $scope.initializeSupplierTreeView = function () {
        var supplierTreeData = $scope.buildTreeData($scope.supplierGroups, 'MASTER_SUPPLIER_CODE', 'PRE_SUPPLIER_CODE', 'SUPPLIER_EDESC', 'SUPPLIER_CODE');
        var ds = new kendo.data.HierarchicalDataSource({
            data: supplierTreeData,
            schema: { model: { children: 'items' } }
        });
        var tv = $("#supplierTreeView").data("kendoTreeView");
        if (tv) { tv.destroy(); $("#supplierTreeView").empty(); }
        $("#supplierTreeView").kendoTreeView({
            dataSource: ds,
            dataTextField: "text",
            checkboxes: { checkChildren: true, threeState: false },
            loadOnDemand: false,
            change: function () {
                if ($scope.currentPartyType === 'supplier') {
                    $scope.refreshGenericSelectedGrid();
                }
            }
        });
    };

    $scope.itemCheckedIds = $scope.itemCheckedIds || [];

    $scope.initializeItemTreeView = function () {
        var itemTreeData = $scope.buildTreeData($scope.itemGroups, 'MASTER_ITEM_CODE', 'PRE_ITEM_CODE', 'ITEM_EDESC', 'ITEM_CODE');

        var ds = new kendo.data.HierarchicalDataSource({
            data: itemTreeData,
            schema: { model: { children: 'items' } }
        });
        var tv2 = $("#itemTreeView").data("kendoTreeView");
        var tvOptions = {
            dataSource: ds,
            dataTextField: "text",
            checkboxes: { checkChildren: true, threeState: false },
            loadOnDemand: false,
            check: function () { $scope.updateItemGridFromTreeSelection(); },
            change: function () { $scope.updateItemGridFromTreeSelection(); }
        };
        if (tv2) { tv2.destroy(); $("#itemTreeView").empty(); }
        $("#itemTreeView").kendoTreeView(tvOptions);

        $timeout(function () {
            var tvRestore = $("#itemTreeView").data("kendoTreeView");
            var ids = $scope.itemCheckedIds || [];
            if (tvRestore && ids.length) {
                var set = {};
                ids.forEach(function (id) { set[id] = true; });
                (function walk(nodes) {
                    (nodes || []).forEach(function (n) {
                        if (set[n.id]) {
                            var li = tvRestore.findByUid(n.uid);
                            if (li && li.length) tvRestore.dataItem(li).set('checked', true);
                        }
                        if (n.items && n.items.length) walk(n.items);
                    });
                })(tvRestore.dataSource.view());
            }
            $scope.updateItemGridFromTreeSelection();
        }, 0);
    };

    $scope.updateItemGridFromTreeSelection = function () {
        var tv = $("#itemTreeView").data("kendoTreeView");
        if (!tv) return;
        var nodes = $scope.getCheckedNodesInfo(tv);
        var masters = nodes.filter(function (n) { return !n.isLeaf; }).map(function (n) { return n.id; });
        var leaves = nodes.filter(function (n) { return n.isLeaf; }).map(function (n) { return { ITEM_CODE: n.originalCode || n.id, ITEM_EDESC: n.text }; });

        (function persistCheckedIds() {
            var ids = nodes.map(function (n) { return n.id; });
            var seen = {}; var uniq = [];
            ids.forEach(function (id) { if (!seen[id]) { seen[id] = true; uniq.push(id); } });
            $scope.itemCheckedIds = uniq;
        })();

        if ($scope.itemSelectMode === 'individual') {
            if (!masters.length) {
                $scope.selectedItems = (leaves || []).map(function (it) { it.selected = false; return it; });
                $scope.refreshSelectedItemsGrid();
                return;
            }
            $http.post('/api/SetupApi/GetIndividualsByPreItemCodes', { MasterCodes: masters })
                .then(function (resp) {
                    var data = (resp.data && (resp.data.DATA || resp.data)) || [];
                    var expanded = data.map(function (x) { return { ITEM_CODE: x.ITEM_CODE || x.CODE || x.ID, ITEM_EDESC: x.ITEM_EDESC || x.NAME || x.EDESC || x.TEXT }; })
                        .filter(function (x) { return !!x.ITEM_CODE; });
                    var merged = expanded.concat(leaves || []);
                    var seen = {}; var unique = [];
                    merged.forEach(function (it) { if (!seen[it.ITEM_CODE]) { seen[it.ITEM_CODE] = true; unique.push(it); } });
                    $scope.selectedItems = unique.map(function (it) { it.selected = false; return it; });
                    $scope.refreshSelectedItemsGrid();
                })
                .catch(function () {
                    $scope.selectedItems = (leaves || []).map(function (it) { it.selected = false; return it; });
                    $scope.refreshSelectedItemsGrid();
                });
            return;
        }

        if (!masters.length) {
            $scope.selectedItems = (leaves || []).map(function (it) { it.selected = false; return it; });
            $scope.refreshSelectedItemsGrid();
            return;
        }
        $http.post('/api/SetupApi/GetIndividualsByPreItemCodes', { MasterCodes: masters })
            .then(function (resp) {
                var data = (resp.data && (resp.data.DATA || resp.data)) || [];
                var expanded = data.map(function (x) { return { ITEM_CODE: x.ITEM_CODE || x.CODE || x.ID, ITEM_EDESC: x.ITEM_EDESC || x.NAME || x.EDESC || x.TEXT }; })
                    .filter(function (x) { return !!x.ITEM_CODE; });
                var merged = expanded.concat(leaves || []);
                var seen = {};
                var unique = [];
                merged.forEach(function (it) { if (!seen[it.ITEM_CODE]) { seen[it.ITEM_CODE] = true; unique.push(it); } });
                $scope.selectedItems = unique.map(function (it) { it.selected = false; return it; });
                $scope.refreshSelectedItemsGrid();
            })
            .catch(function () {
                $scope.selectedItems = (leaves || []).map(function (it) { it.selected = false; return it; });
                $scope.refreshSelectedItemsGrid();
            });
    };

    $scope.$watch('itemSelectMode', function (nv, ov) {
        if (nv === ov) return;
        $scope.updateItemGridFromTreeSelection();
    });

    $scope.buildTreeData = function (data, idField, parentField, textField, originalCodeField) {
        var tree = [];
        var lookup = {};

        angular.forEach(data, function (item) {
            lookup[item[idField]] = {
                id: item[idField],
                text: item[textField],
                parentId: item[parentField],
                items: [],
                hasChildren: false,
                originalCode: originalCodeField ? item[originalCodeField] : undefined
            };
        });

        angular.forEach(lookup, function (item) {
            if (item.parentId && lookup[item.parentId]) {
                lookup[item.parentId].items.push(item);
                lookup[item.parentId].hasChildren = true;
            } else {
                tree.push(item);
            }
        });

        return tree;
    };

    $scope._preCustomerSet = $scope._preCustomerSet || {};
    $scope.buildCustomerHierarchySets = function () {
        var preSet = {};
        angular.forEach($scope.customerGroups || [], function (item) {
            if (item && item.PRE_CUSTOMER_CODE) preSet[item.PRE_CUSTOMER_CODE] = true;
        });
        $scope._preCustomerSet = preSet;
    };
    $scope.isCustomerIndividual = function (code) {
        if (!code) return false;
        if (!$scope._preCustomerSet) return true;
        return !$scope._preCustomerSet[code];
    };

    $scope.getCheckedLeafNodes = function (treeView) {
        var checked = [];
        if (!treeView) return checked;
        function traverse(dataItems) {
            angular.forEach(dataItems, function (di) {
                var hasChildren = di.hasChildren && di.children && di.children.view().length > 0;
                if (di.checked && !hasChildren) {
                    checked.push({ id: di.id, text: di.text, originalCode: di.originalCode });
                }
                if (hasChildren) traverse(di.children.view());
            });
        }
        traverse(treeView.dataSource.view());
        return checked;
    };

    $scope.buildFilteredTree = function (data, idField, parentField, textField, originalCodeField, query) {
        if (!query) return $scope.buildTreeData(data, idField, parentField, textField, originalCodeField);
        query = (query || '').toLowerCase();
        var fullTree = $scope.buildTreeData(data, idField, parentField, textField, originalCodeField);
        function filterNode(node) {
            var keep = (node.text || '').toLowerCase().indexOf(query) !== -1;
            var kept = [];
            angular.forEach(node.items || [], function (child) {
                if (filterNode(child)) kept.push(child);
            });
            node.items = kept;
            node.hasChildren = kept.length > 0;
            return keep || kept.length > 0;
        }
        var result = [];
        angular.forEach(fullTree, function (root) { if (filterNode(root)) result.push(root); });
        return result;
    };

    $scope.filterItemTree = function (query) {
        var tv = $("#itemTreeView").data("kendoTreeView");
        if (!tv) return;
        
        var filteredData = $scope.buildFilteredTree($scope.itemGroups, 'MASTER_ITEM_CODE', 'PRE_ITEM_CODE', 'ITEM_EDESC', 'ITEM_CODE', query);
        var ds = new kendo.data.HierarchicalDataSource({
            data: filteredData,
            schema: { model: { children: 'items' } }
        });
        tv.setDataSource(ds);
    };

    $scope.traverseTree = function (treeView, cb) {
        if (!treeView) return;
        treeView.items().each(function () { $scope._traverseNode(treeView, this, cb); });
    };
    $scope._traverseNode = function (treeView, nodeEl, cb) {
        var dataItem = treeView.dataItem(nodeEl);
        cb(dataItem, nodeEl);
        var children = $(nodeEl).find('> .k-group > .k-item');
        children.each(function () { $scope._traverseNode(treeView, this, cb); });
    };

    $scope.getLeafDescendants = function (dataItem) {
        var res = [];
        function walk(n) {
            if (!n.items || n.items.length === 0) { res.push(n); return; }
            angular.forEach(n.items, walk);
        }
        walk(dataItem);
        return res;
    };

    $scope.customerSelectMode = 'group';
    $scope.customerSelectAll = false;
    $scope.customerGridSelectAll = false;
    $scope.customerSelected = [];

    $scope.dealerSelectMode = 'group';
    $scope.dealerSelectAll = false;
    $scope.dealerGridSelectAll = false;
    $scope.dealerSelected = [];

    $scope.supplierSelectMode = 'group';
    $scope.supplierSelectAll = false;
    $scope.supplierGridSelectAll = false;
    $scope.supplierSelected = [];

    $scope.itemSelectMode = 'group';
    $scope.itemSelectAll = false;
    $scope.itemGridSelectAll = false;

    $scope.selectedCustomers = [];

    $scope.toggleCustomerSelectAll = function () {
        var tv = $("#customerTreeView").data("kendoTreeView");
        if (!tv) return;
        $scope.traverseTree(tv, function (di) { di.set('checked', $scope.customerSelectAll); });
    };
    $scope.toggleDealerSelectAll = function () {
        var tv = $("#dealerTreeView").data("kendoTreeView");
        if (!tv) return;
        $scope.traverseTree(tv, function (di) { di.set('checked', $scope.dealerSelectAll); });
    };
    $scope.toggleSupplierSelectAll = function () {
        var tv = $("#supplierTreeView").data("kendoTreeView");
        if (!tv) return;
        $scope.traverseTree(tv, function (di) { di.set('checked', $scope.supplierSelectAll); });
    };
    $scope.toggleItemSelectAll = function () {
        var tv = $("#itemTreeView").data("kendoTreeView");
        if (!tv) return;
        $scope.traverseTree(tv, function (di) { di.set('checked', $scope.itemSelectAll); });
        $scope.updateItemGridFromTreeSelection();
    };

    $scope.filterCustomerTree = function (q) {
        var filtered = $scope.buildFilteredTree($scope.customerGroups, 'MASTER_CUSTOMER_CODE', 'PRE_CUSTOMER_CODE', 'CUSTOMER_EDESC', 'CUSTOMER_CODE', q);
        var ds = new kendo.data.HierarchicalDataSource({ data: filtered, schema: { model: { children: 'items' } } });
        var tv = $("#customerTreeView").data("kendoTreeView");
        if (tv) { tv.destroy(); $("#customerTreeView").empty(); }
        $("#customerTreeView").kendoTreeView({ dataSource: ds, dataTextField: 'text', checkboxes: { checkChildren: true, threeState: false }, loadOnDemand: false });
    };
    $scope.filterDealerTree = function (q) {
        var filtered = $scope.buildFilteredTree($scope.dealerGroups, 'MASTER_PARTY_CODE', 'PRE_PARTY_CODE', 'PARTY_TYPE_EDESC', 'PARTY_TYPE_CODE', q);
        var ds = new kendo.data.HierarchicalDataSource({ data: filtered, schema: { model: { children: 'items' } } });
        var tv = $("#dealerTreeView").data("kendoTreeView");
        if (tv) { tv.destroy(); $("#dealerTreeView").empty(); }
        $("#dealerTreeView").kendoTreeView({ dataSource: ds, dataTextField: 'text', checkboxes: { checkChildren: true, threeState: false }, loadOnDemand: false });
    };
    $scope.filterSupplierTree = function (q) {
        var filtered = $scope.buildFilteredTree($scope.supplierGroups, 'MASTER_SUPPLIER_CODE', 'PRE_SUPPLIER_CODE', 'SUPPLIER_EDESC', 'SUPPLIER_CODE', q);
        var ds = new kendo.data.HierarchicalDataSource({ data: filtered, schema: { model: { children: 'items' } } });
        var tv = $("#supplierTreeView").data("kendoTreeView");
        if (tv) { tv.destroy(); $("#supplierTreeView").empty(); }
        $("#supplierTreeView").kendoTreeView({ dataSource: ds, dataTextField: 'text', checkboxes: { checkChildren: true, threeState: false }, loadOnDemand: false });
    };
    $scope.filterItemTree = function (q) {
        var filtered = $scope.buildFilteredTree($scope.itemGroups, 'MASTER_ITEM_CODE', 'PRE_ITEM_CODE', 'ITEM_EDESC', 'ITEM_CODE', q);
        var ds = new kendo.data.HierarchicalDataSource({ data: filtered, schema: { model: { children: 'items' } } });
        var tv = $("#itemTreeView").data("kendoTreeView");
        if (tv) { tv.destroy(); $("#itemTreeView").empty(); }
        $("#itemTreeView").kendoTreeView({ dataSource: ds, dataTextField: 'text', checkboxes: { checkChildren: true, threeState: false }, loadOnDemand: false, check: function () { $scope.updateItemGridFromTreeSelection(); }, change: function () { $scope.updateItemGridFromTreeSelection(); } });
        $timeout(function () {
            var tvRestore = $("#itemTreeView").data("kendoTreeView");
            var ids = $scope.itemCheckedIds || [];
            if (tvRestore && ids.length) {
                var set = {}; ids.forEach(function (id) { set[id] = true; });
                (function walk(nodes) {
                    (nodes || []).forEach(function (n) {
                        if (set[n.id]) {
                            var li = tvRestore.findByUid(n.uid);
                            if (li && li.length) tvRestore.dataItem(li).set('checked', true);
                        }
                        if (n.items && n.items.length) walk(n.items);
                    });
                })(tvRestore.dataSource.view());
            }
            $scope.updateItemGridFromTreeSelection();
        }, 0);
    };

    $scope.refreshSelectedItemsGrid = function () {
        var grid = $("#selectedItemsGrid").data("kendoGrid");
        var gridOptions = {
            dataSource: {
                data: $scope.selectedItems,
                schema: { model: { id: 'ITEM_CODE', fields: { ITEM_CODE: { type: 'string' }, ITEM_EDESC: { type: 'string' }, selected: { type: 'boolean', defaultValue: false } } } }
            },
            height: 520,
            scrollable: true,
            columns: [
                { headerTemplate: "<input type='checkbox' class='item-header-check' />", width: 44, attributes: { style: 'text-align:center;' }, headerAttributes: { style: 'text-align:center; width:44px;' }, template: "<input type='checkbox' class='item-row-check' #= selected ? 'checked=checked' : '' # />" },
                { field: 'ITEM_CODE', title: 'Item Code', width: 120 },
                { field: 'ITEM_EDESC', title: 'Item Description' }
            ],
            dataBound: function () {
                var g = this;
                var $grid = $(g.element);

                function updateHeaderState() {
                    var view = g.dataSource.view();
                    if (!view || view.length === 0) { $grid.find('.item-header-check').prop({ checked: false, indeterminate: false }); return; }
                    var total = view.length;
                    var selectedCount = 0;
                    for (var i = 0; i < total; i++) { if (view[i].selected) selectedCount++; }
                    var all = selectedCount === total;
                    var none = selectedCount === 0;
                    var ind = !all && !none;
                    var $hdr = $grid.find('.item-header-check');
                    $hdr.prop('checked', all);
                    $hdr.prop('indeterminate', ind);
                }

                g.tbody.off('change.item').on('change.item', '.item-row-check', function () {
                    var tr = $(this).closest('tr');
                    var di = g.dataItem(tr);
                    if (!di) return;
                    di.set('selected', this.checked);
                    updateHeaderState();
                });

                $grid.find('.item-header-check').off('change.itemAll').on('change.itemAll', function () {
                    var check = this.checked;
                    var view = g.dataSource.view();
                    for (var i = 0; i < view.length; i++) { view[i].set('selected', check); }
                    g.tbody.find('.item-row-check').prop('checked', check);
                    updateHeaderState();
                });

                updateHeaderState();
            }
        };
        if (!grid) {
            $("#selectedItemsGrid").kendoGrid(gridOptions);
        } else {
            grid.setOptions(gridOptions);
            grid.dataSource.data($scope.selectedItems);
            grid.refresh();
        }
    };

    $scope.refreshCustomerSelectedGrid = function () {
        var grid = $("#customerSelectedGrid").data("kendoGrid");
        var gridOptions = {
            dataSource: {
                data: $scope.customerSelected,
                schema: { model: { id: 'CODE', fields: { CODE: { type: 'string' }, NAME: { type: 'string' }, selected: { type: 'boolean', defaultValue: false } } } }
            },
            height: 300,
            scrollable: true,
            columns: [
                { title: "", width: 40, template: "<input type='checkbox' class='customer-row-check' #= selected ? 'checked=checked' : '' # />" },
                { field: 'CODE', title: 'Shortcut', width: 120 },
                { field: 'NAME', title: 'Description' }
            ],
            dataBound: function () {
                var g = this;
                g.tbody.off('change.customer').on('change.customer', '.customer-row-check', function () {
                    var tr = $(this).closest('tr');
                    var di = g.dataItem(tr);
                    di.set('selected', this.checked);
                });
            }
        };
        if (!grid) {
            $("#customerSelectedGrid").kendoGrid(gridOptions);
        } else {
            grid.setOptions(gridOptions);
            grid.dataSource.data($scope.customerSelected);
            grid.refresh();
        }
    };

    $scope.moveCustomersToGrid = function () {
        debugger
        var tv = $("#customerTreeView").data("kendoTreeView");
        if (!tv) return;
        var checkedNodes = $scope.getCheckedNodesInfo(tv);
        if (!checkedNodes.length) { $scope.refreshCustomerSelectedGrid(); return; }
        if ($scope.customerSelectMode === 'individual') {
            var leafNodes = checkedNodes.filter(function (n) { return n.isLeaf; });
            var groupNodes = checkedNodes.filter(function (n) { return !n.isLeaf; });
            var merged = leafNodes.map(function (n) { return { CODE: n.originalCode || n.id, NAME: n.text }; });
            if (groupNodes.length > 0) {
                var gcodes = groupNodes.map(function (g) { return g.id; });
                var promises = gcodes.map(function (gid) {
                    return $http.get('/api/SetupApi/GetCustomersByGroup?groupCode=' + encodeURIComponent(gid))
                        .then(function (resp) {
                            var arr = (resp.data && resp.data.DATA) ? resp.data.DATA : [];
                            var onlyIndividuals = arr.filter(function (x) { return $scope.isCustomerIndividual(x.CUSTOMER_CODE); });
                            Array.prototype.push.apply(merged, onlyIndividuals.map(function (x) { return { CODE: x.CUSTOMER_CODE, NAME: x.CUSTOMER_EDESC }; }));
                        })
                        .catch(function () { });
                });
                Promise.all(promises).then(function () {
                    var cmap = {}; angular.forEach($scope.customerSelected || [], function (c) { cmap[c.CODE] = c; });
                    angular.forEach(merged, function (c) { if (!cmap[c.CODE]) cmap[c.CODE] = c; });
                    $scope.customerSelected = Object.keys(cmap).map(function (k) { return cmap[k]; });
                    $scope.refreshCustomerSelectedGrid();
                    $scope.$applyAsync();
                });
            } else {
                var cmap2 = {}; angular.forEach($scope.customerSelected || [], function (c) { cmap2[c.CODE] = c; });
                angular.forEach(merged, function (c) { if (!cmap2[c.CODE]) cmap2[c.CODE] = c; });
                $scope.customerSelected = Object.keys(cmap2).map(function (k) { return cmap2[k]; });
                $scope.refreshCustomerSelectedGrid();
            }
            return;
        }
        var groups = checkedNodes.filter(function (n) { return !n.isLeaf; });
        var directIndividuals = checkedNodes.filter(function (n) { return n.isLeaf; });

        var treeMap = {}; 
        ($scope.selectedCustomerTreeData || []).forEach(function (n) { treeMap[n.id] = n; });

        directIndividuals.forEach(function (n) {
            var code = n.originalCode || n.id;
            if (!$scope.customerSelected) $scope.customerSelected = [];
            if ($scope.customerSelected.filter(function (x) { return x.CODE === code; }).length === 0) {
                $scope.customerSelected.push({ CODE: code, NAME: n.text });
            }
            if (!treeMap[code]) {
                treeMap[code] = { id: code, text: n.text, hasChildren: false };
            }
        });

        var groupCodes = groups.map(function (g) { return g.id; });
        var groupNameById = {}; groups.forEach(function (g) { groupNameById[g.id] = g.text; });

        var afterMerge = function (list) {
            var map = {};
            ($scope.customerSelected || []).forEach(function (it) { map[it.CODE] = it; });
            list.forEach(function (it) { map[it.CODE] = it; });
            $scope.customerSelected = Object.keys(map).map(function (k) { return map[k]; });

            groupCodes.forEach(function (gid) {
                var children = (list || []).filter(function (it) { return it.GROUP_ID === gid || true; }); // fallback attach all fetched to each group if no group id available
            });
            $scope.refreshCustomerSelectedGrid();
            $scope.$applyAsync();
        };

        if (groupCodes.length === 0) {
            $scope.selectedCustomerTreeData = Object.keys(treeMap).map(function (k) { return treeMap[k]; });
            $scope.refreshCustomerSelectedGrid();
            return;
        }

        var promises = [];
        var perGroupChildren = {};
        groupCodes.forEach(function (gid) {
            promises.push($http.get('/api/SetupApi/GetCustomersByGroup?groupCode=' + encodeURIComponent(gid))
                .then(function (resp) {
                    var arr = (resp.data && resp.data.DATA) ? resp.data.DATA : [];
                    var onlyIndividuals = arr.filter(function (x) { return $scope.isCustomerIndividual(x.CUSTOMER_CODE); });
                    perGroupChildren[gid] = onlyIndividuals.map(function (x) { return { CODE: x.CUSTOMER_CODE, NAME: x.CUSTOMER_EDESC }; });
                })
                .catch(function (err) {  perGroupChildren[gid] = []; }));
        });

        Promise.all(promises).then(function () {
            var merged = [];
            Object.keys(perGroupChildren).forEach(function (gid) { merged = merged.concat(perGroupChildren[gid]); });

            var uniq = {}; var flat = [];
            ($scope.customerSelected || []).concat(merged).forEach(function (it) { if (!uniq[it.CODE]) { uniq[it.CODE] = true; flat.push(it); } });
            $scope.customerSelected = flat;

            Object.keys(perGroupChildren).forEach(function (gid) {
                var node = treeMap[gid] || { id: gid, text: (groupNameById[gid] || gid), items: [], hasChildren: true };
                var childMap = {}; (node.items || []).forEach(function (c) { childMap[c.id] = true; });
                (perGroupChildren[gid] || []).forEach(function (c) {
                    if (!childMap[c.CODE]) {
                        (node.items = node.items || []).push({ id: c.CODE, text: c.NAME, hasChildren: false });
                        childMap[c.CODE] = true;
                    }
                });
                treeMap[gid] = node;
            });

            $scope.selectedCustomerTreeData = Object.keys(treeMap).map(function (k) { return treeMap[k]; });
            $scope.refreshCustomerSelectedGrid();
            $scope.$applyAsync();
        });
    };

    $scope.removeCustomersFromGrid = function () {
        var grid = $("#customerSelectedGrid").data("kendoGrid");
        if (!grid) return;
        if ($scope.customerGridSelectAll) {
            $scope.customerSelected = [];
        } else {
            $scope.customerSelected = ($scope.customerSelected || []).filter(function (x) { return !x.selected; });
        }
        $scope.customerGridSelectAll = false;
        $scope.refreshCustomerSelectedGrid();
        updateSummaries();
    };

    $scope.toggleCustomerGridSelectAll = function () {
        var grid = $("#customerSelectedGrid").data("kendoGrid");
        if (!grid) return;
        var items = grid.dataSource.view();
        for (var i = 0; i < items.length; i++) {
            items[i].set('selected', $scope.customerGridSelectAll);
        }
        grid.refresh();
    };
    $scope.filterCustomerGrid = function (q) {
        var grid = $("#customerSelectedGrid").data("kendoGrid");
        if (!grid) return;
        grid.dataSource.filter(q ? [{ field: 'NAME', operator: 'contains', value: q }] : []);
    };

    $scope.isDealerIndividual = function (dealerCode) {
        if (!dealerCode || !$scope.dealerGroups) return true;
        return !$scope.dealerGroups.some(function (g) { return g.PRE_PARTY_CODE === dealerCode; });
    };

    $scope.toggleDealerSelectAll = function () {
        var tv = $("#dealerTreeView").data("kendoTreeView");
        if (!tv) return;
        $scope.traverseTree(tv, function (di) { di.set('checked', $scope.dealerSelectAll); });
    };

    $scope.toggleDealerGridSelectAll = function () {
        var grid = $("#dealerSelectedGrid").data("kendoGrid");
        if (!grid) return;
        var items = grid.dataSource.view();
        for (var i = 0; i < items.length; i++) {
            items[i].set('selected', $scope.dealerGridSelectAll);
        }
        grid.refresh();
    };

    $scope.filterDealerGrid = function (q) {
        var grid = $("#dealerSelectedGrid").data("kendoGrid");
        if (!grid) return;
        grid.dataSource.filter(q ? [{ field: 'NAME', operator: 'contains', value: q }] : []);
    };

    $scope.isSupplierIndividual = function (supplierCode) {
        if (!supplierCode || !$scope.supplierGroups) return true;
        return !$scope.supplierGroups.some(function (g) { return g.PRE_SUPPLIER_CODE === supplierCode; });
    };

    $scope.toggleSupplierSelectAll = function () {
        var tv = $("#supplierTreeView").data("kendoTreeView");
        if (!tv) return;
        $scope.traverseTree(tv, function (di) { di.set('checked', $scope.supplierSelectAll); });
    };

    $scope.toggleItemSelectAll = function () {
        var tv = $("#itemTreeView").data("kendoTreeView");
        if (!tv) return;
        $scope.traverseTree(tv, function (di) { di.set('checked', $scope.itemSelectAll); });
    };

    $scope.toggleSupplierGridSelectAll = function () {
        var grid = $("#supplierSelectedGrid").data("kendoGrid");
        if (!grid) return;
        var items = grid.dataSource.view();
        for (var i = 0; i < items.length; i++) {
            items[i].set('selected', $scope.supplierGridSelectAll);
        }
        grid.refresh();
    };

    $scope.filterSupplierGrid = function (q) {
        var grid = $("#supplierSelectedGrid").data("kendoGrid");
        if (!grid) return;
        grid.dataSource.filter(q ? [{ field: 'NAME', operator: 'contains', value: q }] : []);
    };

    $scope.moveItemsToGrid = function () {
        var tv = $("#itemTreeView").data("kendoTreeView");
        if (!tv) return;
        var checked = $scope.getCheckedNodesInfo(tv);
        if (!checked.length) { $scope.refreshSelectedItemsGrid(); return; }
        var individuals = checked.filter(function (n) { return n.isLeaf; });
        var masters = checked.filter(function (n) { return !n.isLeaf; });
        var mergedList = [];
        Array.prototype.push.apply(mergedList, individuals.map(function (n) { return { ITEM_CODE: n.originalCode || n.id, ITEM_EDESC: n.text }; }));
        if (masters.length > 0) {
            var codes = masters.map(function (m) { return m.id; });
            $http.post('/api/SetupApi/GetIndividualsByPreItemCodes', { MasterCodes: codes })
                .then(function (resp) {
                    var data = (resp.data && resp.data.DATA) ? resp.data.DATA : [];
                    Array.prototype.push.apply(mergedList, data.map(function (x) { return { ITEM_CODE: x.ITEM_CODE, ITEM_EDESC: x.ITEM_EDESC }; }));
                })
                .catch(function (err) {  })
                .finally(function () {
                    var map = {}; angular.forEach($scope.selectedItems || [], function (it) { map[it.ITEM_CODE] = it; });
                    angular.forEach(mergedList, function (it) { map[it.ITEM_CODE] = map[it.ITEM_CODE] || it; });
                    $scope.selectedItems = Object.keys(map).map(function (k) { return map[k]; });
                    $scope.refreshSelectedItemsGrid();
                    $scope.$applyAsync();
                });
        } else {
            var map2 = {}; angular.forEach($scope.selectedItems || [], function (it) { map2[it.ITEM_CODE] = it; });
            angular.forEach(mergedList, function (it) { map2[it.ITEM_CODE] = map2[it.ITEM_CODE] || it; });
            $scope.selectedItems = Object.keys(map2).map(function (k) { return map2[k]; });
            $scope.refreshSelectedItemsGrid();
        }
    };
    $scope.removeItemsFromGrid = function () {
        var grid = $("#selectedItemsGrid").data("kendoGrid");
        if (!grid) return;
        if ($scope.itemGridSelectAll) {
            $scope.selectedItems = [];
        } else {
            $scope.selectedItems = ($scope.selectedItems || []).filter(function (x) { return !x.selected; });
        }
        $scope.itemGridSelectAll = false;
        grid.dataSource.data($scope.selectedItems);
        updateSummaries();
    };

    $scope.toggleItemGridSelectAll = function () {
        var grid = $("#selectedItemsGrid").data("kendoGrid");
        if (!grid) return;
        var items = grid.dataSource.view();
        for (var i = 0; i < items.length; i++) {
            items[i].set('selected', $scope.itemGridSelectAll);
        }
        grid.refresh();
    };

    $scope.clearSelections = function () {
        $scope.customerSelected = [];
        $scope.selectedCustomers = [];
        $scope.refreshCustomerSelectedGrid();
        var stv = $("#selectedCustomersTree").data("kendoTreeView");
        if (stv) stv.setDataSource(new kendo.data.HierarchicalDataSource({ data: [] }));
        $scope.selectedItems = [];
        var igrid = $("#selectedItemsGrid").data("kendoGrid");
        if (igrid) igrid.dataSource.data($scope.selectedItems);
        updateSummaries();
    };

    $scope.calculateNetRate = function (model) {
        var standardRate = parseFloat(model.STANDARD_RATE || 0);
        var discountPercent = parseFloat(model.DISCOUNT_PERCENT || 0);
        var discountAmount = (standardRate * discountPercent) / 100;
        model.NET_RATE = standardRate - discountAmount;
    };

    $scope.openItemModal = function () {
        if (!$("#itemTreeView").data("kendoTreeView")) {
            $scope.initializeItemTreeView();
        }
        if (Array.isArray($scope.selectedItems)) {
            for (var i = 0; i < $scope.selectedItems.length; i++) {
                if ($scope.selectedItems[i]) $scope.selectedItems[i].selected = false;
            }
        }
        $('#itemModal').modal('show');
        $scope.refreshSelectedItemsGrid();
        
        $timeout(function() {
            $("#itemGridSearch").off('input.itemGridSearch').on('input.itemGridSearch', function () {
                var q = (this.value || '').toString().trim();
                $scope.filterItemGrid(q);
            });
        }, 100);
    };

    $scope.selectItems = function () {
        if ($scope.itemSelectMode === 'group') {
            var tv = $("#itemTreeView").data("kendoTreeView");
            if (!tv) { $('#itemModal').modal('hide'); return; }
            var nodes = $scope.getCheckedNodesInfo(tv);
            var masters = nodes.filter(function (n) { return !n.isLeaf; });
            var leaves = nodes.filter(function (n) { return n.isLeaf; });
            var items = leaves.map(function (n) { return { ITEM_CODE: n.originalCode || n.id, ITEM_EDESC: n.text }; });
            var codes = masters.map(function (m) { return m.id; });
            if (codes.length === 0 && items.length === 0) { $('#itemModal').modal('hide'); updateSummaries(); return; }
            if (codes.length > 0) {
                $http.post('/api/SetupApi/GetIndividualsByPreItemCodes', { MasterCodes: codes })
                    .then(function (resp) {
                        var data = (resp.data && resp.data.DATA) ? resp.data.DATA : [];
                        Array.prototype.push.apply(items, data.map(function (x) { return { ITEM_CODE: x.ITEM_CODE, ITEM_EDESC: x.ITEM_EDESC }; }));
                    })
                    .finally(function () {
                        $scope.selectedItems = (items || []).map(function (it) { it.selected = true; return it; });
                        $scope.refreshSelectedItemsGrid();
                        $scope.loadItemRates(items);
                        $('#itemModal').modal('hide');
                        updateSummaries();
                    });
            } else {
                $scope.selectedItems = (items || []).map(function (it) { it.selected = true; return it; });
                $scope.refreshSelectedItemsGrid();
                $scope.loadItemRates(items);
                $('#itemModal').modal('hide');
                updateSummaries();
            }
        } else {
            var chosen = [];
            var igrid = $("#selectedItemsGrid").data("kendoGrid");
            if (igrid) {
                var view = igrid.dataSource.view();
                for (var i = 0; i < view.length; i++) { if (view[i].selected) chosen.push(view[i]); }
            } else {
                chosen = ($scope.selectedItems || []).filter(function (x) { return x.selected; });
            }
            if (chosen.length === 0) {
                var tv = $("#itemTreeView").data("kendoTreeView");
                if (tv) {
                    var nodes = $scope.getCheckedNodesInfo(tv);
                    var masters = nodes.filter(function (n) { return !n.isLeaf; });
                    var leaves = nodes.filter(function (n) { return n.isLeaf; });
                    var items = leaves.map(function (n) { return { ITEM_CODE: n.originalCode || n.id, ITEM_EDESC: n.text }; });
                    var codes = masters.map(function (m) { return m.id; });
                    if (codes.length > 0) {
                        $http.post('/api/SetupApi/GetIndividualsByPreItemCodes', { MasterCodes: codes })
                            .then(function (resp) {
                                var data = (resp.data && resp.data.DATA) ? resp.data.DATA : [];
                                Array.prototype.push.apply(items, data.map(function (x) { return { ITEM_CODE: x.ITEM_CODE, ITEM_EDESC: x.ITEM_EDESC }; }));
                            })
                            .finally(function () {
                                $scope.selectedItems = (items || []).map(function (it) { it.selected = true; return it; });
                                $scope.refreshSelectedItemsGrid();
                                $scope.loadItemRates(items);
                                $('#itemModal').modal('hide');
                                updateSummaries();
                            });
                    } else {
                        $scope.selectedItems = (items || []).map(function (it) { it.selected = true; return it; });
                        $scope.refreshSelectedItemsGrid();
                        $scope.loadItemRates(items);
                        $('#itemModal').modal('hide');
                        updateSummaries();
                    }
                    return;
                }
            } else {
                $scope.loadItemRates(chosen);
            }
            $('#itemModal').modal('hide');
            updateSummaries();
        }
    };

    $scope.removeSelectedItem = function (e) {
        var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
        var index = $scope.selectedItems.indexOf(dataItem);
        if (index > -1) {
            $scope.selectedItems.splice(index, 1);
            $("#selectedItemsGrid").data("kendoGrid").dataSource.remove(dataItem);
        }
    };

    $scope.loadItemRates = function (chosenItems) {
        $scope.rateSchedule.rateData = [];
        var itemsToLoad = chosenItems && chosenItems.length ? chosenItems : [];
        if (itemsToLoad.length === 0) return;

        var itemCodes = itemsToLoad.map(function (item) { return item.ITEM_CODE; });
        var batchRatesUrl = '/api/SetupApi/GetItemRatesBatch?itemCodes=' + encodeURIComponent(itemCodes.join(','));

        $http.get(batchRatesUrl).then(function (response) {
            var ratesData = (response.data && response.data.DATA) ? response.data.DATA : [];
            var ratesMap = {};

            ratesData.forEach(function (r) {
                var itemCode = r.ITEM_CODE || r.CODE;
                if (itemCode) {
                    ratesMap[itemCode] = {
                        STANDARD_RATE: r.STANDARD_RATE,
                        MRP_RATE: r.MRP_RATE,
                        RETAIL_PRICE: r.RETAIL_PRICE,
                        MU_CODE: r.MU_CODE
                    };
                }
            });
            var missingMUItems = itemsToLoad.filter(function (it) {
                var rr = ratesMap[it.ITEM_CODE] || {};
                return !rr.MU_CODE;
            }).map(function (it) { return it.ITEM_CODE; });

            var finalizeWithData = function (muMap) {
                itemsToLoad.forEach(function (item) {
                    var rates = ratesMap[item.ITEM_CODE] || {};
                    var mu = rates.MU_CODE || (muMap && muMap[item.ITEM_CODE]) || '';
                    var rateItem = {
                        ITEM_CODE: item.ITEM_CODE,
                        ITEM_EDESC: item.ITEM_EDESC,
                        MU_CODE: mu,
                        STANDARD_RATE: rates.STANDARD_RATE || 0,
                        MRP_RATE: rates.MRP_RATE || 0,
                        RETAIL_PRICE: rates.RETAIL_PRICE || 0
                    };
                    $scope.rateSchedule.rateData.push(rateItem);
                });

                var grid = $("#rateGrid").data("kendoGrid");
                if (grid) grid.dataSource.data($scope.rateSchedule.rateData);
            };

            if (missingMUItems.length > 0) {
                var muUrl = '/api/SetupApi/GetMuDescriptionBatch?itemCodes=' + encodeURIComponent(missingMUItems.join(','));
                $http.get(muUrl).then(function (muResp) {
                    var muData = (muResp.data && (muResp.data.DATA || muResp.data.Data)) ? (muResp.data.DATA || muResp.data.Data) : (Array.isArray(muResp.data) ? muResp.data : []);
                    var muMap = {};
                    (muData || []).forEach(function (row) {
                        var code = row.ITEM_CODE || row.CODE || row.Id;
                        var mu = row.MU_CODE || row.MUCode;
                        if (code && mu && !muMap[code]) muMap[code] = mu; 
                    });
                    finalizeWithData(muMap);
                }).catch(function () {
                    finalizeWithData({});
                });
            } else {
                finalizeWithData({});
            }
        });
    };

    $scope.saveRateSchedule = function () {
        var notify = function (msg, type) {
            try {
                if (typeof window.displayPopupNotification === 'function') {
                    window.displayPopupNotification(msg, type || 'success');
                } else {
                    alert(msg);
                }
            } catch (e) { alert(msg); }
        };

        try {
            var grid = $("#rateGrid").data("kendoGrid");
            if (grid) {
                try { grid.saveChanges(); } catch (e1) {}
                var dsData = grid.dataSource && grid.dataSource.data ? grid.dataSource.data() : [];
                var updated = [];
                for (var i = 0; i < dsData.length; i++) {
                    var m = dsData[i];
                    updated.push({
                        ITEM_CODE: m.ITEM_CODE,
                        ITEM_EDESC: m.ITEM_EDESC,
                        MU_CODE: m.MU_CODE,
                        STANDARD_RATE: Number(m.STANDARD_RATE) || 0,
                        MRP_RATE: Number(m.MRP_RATE) || 0,
                        RETAIL_PRICE: Number(m.RETAIL_PRICE) || 0
                    });
                }
                $scope.rateSchedule.rateData = updated;
            }
        } catch (syncErr) { }

        if (!$scope.rateSchedule.effectiveDate) {
            notify('Please select an effective date.', 'warning');
            return;
        }

        if ($scope.rateSchedule.rateData.length === 0) {
            notify('Please select items for rate schedule.', 'warning');
            return;
        }

        var partyType = $scope.rateSchedule.partyType || 'customer';
        var csFlag = partyType === 'dealer' ? 'P' : (partyType === 'supplier' ? 'S' : 'C');

        var selectedArrayKey = partyType === 'dealer' ? 'dealerSelected' : (partyType === 'supplier' ? 'supplierSelected' : 'customerSelected');
        var selectedList = ($scope[selectedArrayKey] || []).filter(function (x) { return x && x.selected; });

        if (!selectedList.length) {
            var fallbackCode = ($scope.rateSchedule.customerCode || '') || ($scope.rateSchedule.partyCode || '');
            if (!fallbackCode) {
                notify('Please select at least one ' + partyType + ' (individual).', 'warning');
                return;
            }
            selectedList = [{ CODE: fallbackCode }];
        }

        var common = {
            EffectiveDate: $scope.rateSchedule.effectiveDate,
            CurrencyCode: $scope.rateSchedule.currencyCode,
            ExchangeRate: $scope.rateSchedule.exchangeRate,
            AreaCode: $scope.rateSchedule.areaCode,
            DocumentCode: $scope.rateSchedule.documentCode,
            CsFlag: csFlag,
            RateData: $scope.rateSchedule.rateData
        };

        var partyCodes = selectedList
            .map(function (p) { return p && (p.CODE || p.CustomerCode || p.PARTY_TYPE_CODE || p.SUPPLIER_CODE || p.id || p.code); })
            .filter(function (c) { return !!c; });

        if (!partyCodes.length) {
            notify('No valid ' + partyType + ' code(s) selected.', 'warning');
            return;
        }

        var payload = angular.extend({}, common, { PartyCodes: partyCodes });

        $http.post('/api/SetupApi/SaveRateSchedule', payload)
            .then(function (resp) {
                var ok = resp && resp.data && (resp.data.STATUS_CODE === 200 || resp.data.StatusCode === 200);
                var msg = (resp && resp.data && (resp.data.MESSAGE || resp.data.Message)) || ('Rate Schedule saved for ' + partyCodes.length + ' ' + partyType + '(s).');
                if (ok) {
                    notify(msg, 'success');
                } else {
                    notify(msg || 'Failed to save rate schedule.', 'error');
                }
            })
            .catch(function (err) {
                var emsg = (err && err.data && (err.data.MESSAGE || err.data.Message)) || 'Failed to save rate schedule.';
                notify(emsg, 'error');
            });
    };

    $scope.refreshData = function () {
        $scope.rateSchedule = {
            effectiveDate: new Date(),
            currencyCode: '',
            exchangeRate: 1,
            areaCode: '',
            customerCode: '',
            customerName: '',
            documentCode: '',
            rateData: []
        };

        $scope.selectedItems = [];
        $("#rateGrid").data("kendoGrid").dataSource.data([]);
        $("#selectedItemsGrid").data("kendoGrid").dataSource.data([]);

        $scope.loadDefaultCurrency();
    };

    $scope.init();
});
