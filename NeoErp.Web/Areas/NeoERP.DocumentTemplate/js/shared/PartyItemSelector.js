    (function (global, $) {
    'use strict';

    // Fetch item group list and build a HierarchicalDataSource
    function fetchItemHierarchyDataSource() {
        var url = '/api/SetupApi/GetItemGroups';
        var map = { idField: 'MASTER_ITEM_CODE', parentField: 'PRE_ITEM_CODE', textField: 'ITEM_EDESC' };
        return $.ajax({ url: url, type: 'GET' }).then(function (resp) {
            var list = Array.isArray(resp) ? resp : (resp.DATA || resp.Data || resp.data || []);
            var nodes = buildHierarchy(list, map);
            return new kendo.data.HierarchicalDataSource({ data: nodes });
        });
    }

    var partyConfigs = {
        customer: {
            modalId: '#customerModal',
            treeViewId: '#customerTreeView',
            selectedGridId: '#customerSelectedGrid',
            selectModeKey: 'customerSelectMode',
            selectAllKey: 'customerSelectAll',
            individualsApiUrl: '/api/SetupApi/GetChildOfCustomerByGroup',
            genericIndividualsByMasters: '/api/SetupApi/GetIndividualsByMasterCodes',
            // Use individual customer fields so grid lists actual customers
            idField: 'CUSTOMER_CODE',
            codeField: 'CUSTOMER_CODE',
            nameField: 'CUSTOMER_EDESC'
        },
        dealer: {
            modalId: '#dealerModal',
            treeViewId: '#dealerTreeView',
            selectedGridId: '#dealerSelectedGrid',
            selectModeKey: 'dealerSelectMode',
            selectAllKey: 'dealerSelectAll',
            individualsApiUrl: '/api/SetupApi/GetChildOfDealerByGroup',
            genericIndividualsByMasters: '/api/SetupApi/GetIndividualsByMasterCodes',
            // Align with the rest of the app using PARTY_TYPE_* fields
            idField: 'PARTY_TYPE_CODE',
            codeField: 'PARTY_TYPE_CODE',
            nameField: 'PARTY_TYPE_EDESC'
        },
        supplier: {
            modalId: '#supplierModal',
            treeViewId: '#supplierTreeView',
            selectedGridId: '#supplierSelectedGrid',
            selectModeKey: 'supplierSelectMode',
            selectAllKey: 'supplierSelectAll',
            individualsApiUrl: '/api/SetupApi/GetChildOfsupplierByGroup',
            genericIndividualsByMasters: '/api/SetupApi/GetIndividualsByMasterCodes',
            idField: 'SUPPLIER_CODE',
            codeField: 'SUPPLIER_CODE',
            nameField: 'SUPPLIER_EDESC'
        }
    };

    var itemConfig = {
        modalId: '#itemModal',
        treeViewId: '#itemTreeView',
        selectedGridId: '#selectedItemsGrid',
        selectModeKey: 'itemSelectMode',
        selectAllKey: 'itemSelectAll',
        individualsApiUrl: '/api/SetupApi/GetItemsByGroup',
        genericIndividualsByMasters: '/api/SetupApi/GetIndividualsByPreItemCodes',
        idField: 'ITEM_CODE',
        codeField: 'ITEM_CODE',
        nameField: 'ITEM_EDESC'
    };

    // Persistent modal state per party type: retains checked groups and selected individuals across opens
    var partyModalState = {
        customer: { groups: new Set(), individuals: new Set() },
        dealer: { groups: new Set(), individuals: new Set() },
        supplier: { groups: new Set(), individuals: new Set() },
        item: { groups: new Set(), individuals: new Set() }
    };

    function getSelectedSetFor(partyType) {
        var st = partyModalState[partyType];
        return (st && st.individuals) ? st.individuals : new Set();
    }

    function getGroupSetFor(partyType) {
        var st = partyModalState[partyType];
        return (st && st.groups) ? st.groups : new Set();
    }

    function updateHeaderCheckboxState($grid) {
        var $rows = $grid.find('.k-grid-content tr');
        var total = $rows.length;
        var checked = $grid.find('.k-grid-content .grid-row-chk:checked').length;
        var $hdr = $grid.find('.k-grid-header .grid-header-chk');
        if (total === 0) { 
            $hdr.prop({ checked: false, indeterminate: false }); 
            return; 
        }
        if (checked === 0) { 
            $hdr.prop({ checked: false, indeterminate: false }); 
            return; 
        }
        if (checked === total) { 
            $hdr.prop({ checked: true, indeterminate: false }); 
            return; 
        }
        $hdr.prop({ checked: false, indeterminate: true });
    }

    function nodeIsGroup(n) {
        return n && (n.hasChildren === true || (Array.isArray(n.items) && n.items.length > 0));
    }

    function getCheckedNodesInfo(tv) {
        var result = [];
        
        // Method 1: Try to get checked nodes from data model
        var flat = (tv && tv.dataSource && typeof tv.dataSource.flatView === 'function') ? tv.dataSource.flatView() : [];
        var checked = flat.filter(function (n) { return n && n.checked; });
        
        // Method 2: If no checked nodes found in data model, inspect DOM directly
        if (checked.length === 0) {
            console.log('[DEBUG] No checked nodes in data model, inspecting DOM...');
            try {
                var treeElement = tv.element;
                treeElement.find('input[type="checkbox"]:checked').each(function(){
                    var $item = $(this).closest('.k-item');
                    var dataItem = tv.dataItem($item);
                    if (dataItem) {
                        checked.push(dataItem);
                    }
                });
                console.log('[DEBUG] Found checked nodes via DOM:', checked.length);
            } catch(e) {
                console.warn('[DEBUG] DOM inspection failed:', e);
            }
        }
        
        checked.forEach(function (n) {
            var isGroup = nodeIsGroup(n);
            var id;
            if (isGroup) {
                // Prefer master code for groups to feed Pre* endpoints
                id = n.MASTER_CODE || n.CODE || n.GROUP_CODE || n.groupCode || n.masterCode;
            } else {
                // Leaves: prefer specific entity codes
                id = n.ITEM_CODE || n.CUSTOMER_CODE || n.DEALER_CODE || n.SUPPLIER_CODE || n.CODE || n.id;
            }
            result.push({
                id: id,
                text: n.text || n.NAME || n.ITEM_EDESC || n.CUSTOMER_EDESC || n.DEALER_NAME || n.SUPPLIER_EDESC || '',
                isLeaf: !isGroup
            });
        });
        
        console.log('[DEBUG] getCheckedNodesInfo result:', result.map(function(r){ return {id: r.id, text: r.text, isLeaf: r.isLeaf}; }));
        return result;
    }

    function updateGridWithData(gridId, fields, data) {
        var grid = $(gridId).data('kendoGrid');
        var isItemGrid = (fields === itemConfig);
        // Normalize data types for filtering (ensure string contains works)
        var normData = (Array.isArray(data) ? data : []).map(function (r) {
            var o = $.extend({}, r);
            if (o[fields.codeField] != null) o[fields.codeField] = o[fields.codeField].toString();
            if (o[fields.nameField] != null) o[fields.nameField] = o[fields.nameField].toString();
            return o;
        });
        // If grid exists but doesn't have header checkbox for party grids, destroy and recreate
        if (grid) {
            var hasHeaderChk = $(gridId).closest('.k-grid').find('.k-grid-header .grid-header-chk').length > 0;
            if (!hasHeaderChk) {
                grid.destroy();
                $(gridId).empty();
                grid = null;
            }
        }
        if (!grid) {
            var columns = [];
            columns.push({
                headerTemplate: '<input type="checkbox" class="grid-header-chk" />',
                template: '<input type="checkbox" class="grid-row-chk" />',
                width: 44,
                headerAttributes: { style: 'text-align:center; width:44px;' },
                attributes: { style: 'text-align:center;' },
                headerCssClass: 'grid-chk-col',
                cssClass: 'grid-chk-col'
            });
            columns.push({ field: fields.codeField, title: fields === itemConfig ? 'Item Code' : (fields === partyConfigs.customer ? 'Customer Code' : (fields === partyConfigs.dealer ? 'Dealer Code' : 'Supplier Code')), width: 120 });
            columns.push({ field: fields.nameField, title: fields === itemConfig ? 'Item Description' : (fields === partyConfigs.customer ? 'Customer Name' : (fields === partyConfigs.dealer ? 'Dealer Name' : 'Supplier Name')) });

            $(gridId).kendoGrid({
                dataSource: { data: normData },
                // Let the container control height; grid content will scroll inside
                height: 520,
                scrollable: true,
                selectable: 'multiple, row',
                columns: columns,
                dataBound: function () {
                    var $grid = $(gridId);
                    var $headerChk = $grid.find('.k-grid-header .grid-header-chk');
                    var partyType = (fields === partyConfigs.customer) ? 'customer' : (fields === partyConfigs.dealer ? 'dealer' : (fields === partyConfigs.supplier ? 'supplier' : (fields === itemConfig ? 'item' : null)));
                    var idField = fields.idField;
                    var selectedSet = getSelectedSetFor(partyType);

                    // Apply checked state to rows based on selected set
                    $grid.find('.k-grid-content tr').each(function () {
                        var dataItem = $(gridId).data('kendoGrid').dataItem(this);
                        if (!dataItem) return;
                        var id = dataItem[idField];
                        var checked = selectedSet.has(id);
                        $(this).find('.grid-row-chk').prop('checked', checked);
                    });

                    // Header toggles all visible
                    $headerChk.off('change.gridAll').on('change.gridAll', function () {
                        var checked = this.checked;
                        $grid.find('.k-grid-content tr').each(function () {
                            var dataItem = $(gridId).data('kendoGrid').dataItem(this);
                            if (!dataItem) return;
                            var id = dataItem[idField];
                            if (checked) selectedSet.add(id); else selectedSet.delete(id);
                            $(this).find('.grid-row-chk').prop('checked', checked);
                        });
                        updateHeaderCheckboxState($grid);
                    });

                    // Row checkbox handler
                    $grid.find('.k-grid-content .grid-row-chk').off('change.rowChk').on('change.rowChk', function () {
                        var tr = $(this).closest('tr');
                        var dataItem = $(gridId).data('kendoGrid').dataItem(tr);
                        if (!dataItem) return;
                        var id = dataItem[idField];
                        if (this.checked) selectedSet.add(id); else selectedSet.delete(id);
                        updateHeaderCheckboxState($grid);
                    });

                    // Initialize header checkbox state
                    updateHeaderCheckboxState($grid);
                    
                    // Ensure grid reflects tree selection state
                    // Resolve current context (customer/dealer/supplier/item)
                    var ctx = (fields === partyConfigs.customer) ? partyConfigs.customer
                             : (fields === partyConfigs.dealer) ? partyConfigs.dealer
                             : (fields === partyConfigs.supplier) ? partyConfigs.supplier
                             : (fields === itemConfig) ? itemConfig
                             : null;
                    // Reflect the persistent selectedSet only; do NOT auto-select based on group checks
                    $grid.find('.k-grid-content tr').each(function() {
                        var dataItem = $(gridId).data('kendoGrid').dataItem(this);
                        if (!dataItem) return;
                        var rowId = dataItem[idField];
                        var isChecked = selectedSet.has(rowId);
                        $(this).find('.grid-row-chk').prop('checked', isChecked);
                    });
                    updateHeaderCheckboxState($grid);
                }
            });
        } else {
            grid.dataSource.data(normData || []);
            // After data update, trigger databound-like sync manually
            var kgrid = $(gridId).data('kendoGrid');
            if (kgrid) {
                // Small timeout to ensure rows are rendered
                setTimeout(function(){ kgrid.trigger('dataBound'); }, 0);
            }
        }
    }

    // Build hierarchical data (array of nodes) from flat group rows
    function buildHierarchy(rows, map) {
        map = map || {}; // { idField, parentField, textField }
        var idF = map.idField, pF = map.parentField, tF = map.textField;
        var byId = {};
        var roots = [];
        (rows || []).forEach(function (r) {
            var id = r[idF];
            var pid = r[pF];
            var text = r[tF] || r.EDESC || r.Edesc || r.NAME || id;
            var node = byId[id] || { items: [] };
            node.id = id;
            node.MASTER_CODE = id; // used by getCheckedNodesInfo for group nodes
            node.text = text;
            node.hasChildren = true;
            byId[id] = node;
            // Defer linking to parent until we process all
            node._parentId = pid;
        });
        Object.keys(byId).forEach(function (id) {
            var n = byId[id];
            var pid = n._parentId;
            if (pid && byId[pid] && pid !== id) {
                byId[pid].items.push(n);
            } else {
                roots.push(n);
            }
            delete n._parentId;
        });
        return roots;
    }

    // Fetch party group list and build a HierarchicalDataSource
    function fetchPartyHierarchyDataSource(partyType) {
        var cfg = partyConfigs[partyType];
        if (!cfg) return $.Deferred().reject('Unknown party type').promise();
        var url, map;
        if (partyType === 'customer') {
            url = '/api/SetupApi/GetCustomerGroups';
            map = { idField: 'MASTER_CUSTOMER_CODE', parentField: 'PRE_CUSTOMER_CODE', textField: 'CUSTOMER_EDESC' };
        } else if (partyType === 'dealer') {
            url = '/api/SetupApi/GetDealerGroups';
            map = { idField: 'MASTER_PARTY_CODE', parentField: 'PRE_PARTY_CODE', textField: 'PARTY_TYPE_EDESC' };
        } else if (partyType === 'supplier') {
            url = '/api/SetupApi/GetSupplierGroups';
            map = { idField: 'MASTER_SUPPLIER_CODE', parentField: 'PRE_SUPPLIER_CODE', textField: 'SUPPLIER_EDESC' };
        }
        if (!url) return $.Deferred().reject('No URL for party type').promise();
        return $.ajax({ url: url, type: 'GET' }).then(function (resp) {
            var list = Array.isArray(resp) ? resp : (resp.DATA || resp.Data || resp.data || []);
            var nodes = buildHierarchy(list, map);
            return new kendo.data.HierarchicalDataSource({ data: nodes });
        });
    }

    function wireGridControlsForParty(partyType) {
        var cfg = partyConfigs[partyType];
        if (!cfg) return;
        var grid = $(cfg.selectedGridId).data('kendoGrid');
        var searchId = '#' + partyType + 'GridSearch'; // e.g., #customerGridSearch

        // Enhanced search filter - matches the working implementation from item modal
        $(searchId).off('input.partyGridSearch').on('input.partyGridSearch', function () {
            var g = $(cfg.selectedGridId).data('kendoGrid');
            if (!g) return;
            var q = (this.value || '').toString().trim();
            if (!q) {
                g.dataSource.filter([]);
                // Refresh header state after clearing filter
                updateHeaderCheckboxState($(cfg.selectedGridId).closest('.k-grid'));
                return;
            }
            g.dataSource.filter({
                logic: 'or',
                filters: [
                    { field: cfg.codeField, operator: 'contains', value: q },
                    { field: cfg.nameField, operator: 'contains', value: q }
                ]
            });
            // Update header state after filtering
            updateHeaderCheckboxState($(cfg.selectedGridId).closest('.k-grid'));
        });
    }

    function fetchIndividualsByMasters(apiUrl, masterCodes, context) {
        if (!masterCodes || masterCodes.length === 0) return $.Deferred().resolve([]).promise();
        var body;
        // If this is the party genericIndividualsByMasters endpoint, include PartyType and CsFlag for completeness
        var isPartyGeneric = /GetIndividualsByMasterCodes$/i.test(apiUrl);
        if (/PreCustomer/i.test(apiUrl)) {
            body = { PreCustomerCodes: masterCodes };
        } else if (/PreItem/i.test(apiUrl)) {
            body = { PreItemCodes: masterCodes };
        } else if (isPartyGeneric && context && (context === 'customer' || context === 'dealer' || context === 'supplier')) {
            var csFlag = context === 'customer' ? 'C' : (context === 'dealer' ? 'P' : 'S');
            body = { PartyType: context, CsFlag: csFlag, MasterCodes: masterCodes };
        } else {
            body = { MasterCodes: masterCodes };
        }
        return $.ajax({
            url: apiUrl,
            type: 'POST',
            data: JSON.stringify(body),
            contentType: 'application/json; charset=utf-8'
        });
    }

    function wireGridControlsForItems() {
        var searchId = '#itemGridSearch';
        var gridId = itemConfig.selectedGridId;
        $(searchId).off('input.itemGridSearch').on('input.itemGridSearch', function () {
            var g = $(gridId).data('kendoGrid');
            if (!g) return;
            var q = (this.value || '').toString().trim();
            if (!q) {
                g.dataSource.filter([]);
                updateHeaderCheckboxState($(gridId).closest('.k-grid'));
                return;
            }
            g.dataSource.filter({
                logic: 'or',
                filters: [
                    { field: itemConfig.codeField, operator: 'contains', value: q },
                    { field: itemConfig.nameField, operator: 'contains', value: q }
                ]
            });
            updateHeaderCheckboxState($(gridId).closest('.k-grid'));
        });
    }

        function refreshSelectedGridForParty(partyType) {
            debugger
        var cfg = partyConfigs[partyType];
        var tv = $(cfg.treeViewId).data('kendoTreeView');
        if (!tv) { updateGridWithData(cfg.selectedGridId, cfg, []); return; }
        var nodes = getCheckedNodesInfo(tv);
        var groups = nodes.filter(function (n) { return n && !n.isLeaf; });
        var leaves = nodes.filter(function (n) { return n && n.isLeaf; });

        if (groups.length === 0 && leaves.length === 0) {
            updateGridWithData(cfg.selectedGridId, cfg, []);
            return;
        }

        // Clean implementation: fetch individuals for all descendant groups under checked parents
        (function(){
            var tv = $(cfg.treeViewId).data('kendoTreeView');
            var flat = tv && tv.dataSource && typeof tv.dataSource.flatView === 'function' ? tv.dataSource.flatView() : [];
            
            function getMasterFromNode(n){
                return (cfg === partyConfigs.customer ? (n.MASTER_CUSTOMER_CODE || n.MASTER_CODE)
                    : cfg === partyConfigs.dealer ? (n.MASTER_PARTY_CODE || n.MASTER_CODE)
                    : cfg === partyConfigs.supplier ? (n.MASTER_SUPPLIER_CODE || n.MASTER_CODE)
                    : (n.MASTER_CODE)) || n.id || n.CODE || n.groupCode || n.masterCode;
            }
            
            // Strategy: For each group from getCheckedNodesInfo, collect all descendant group master codes
            // Backend only returns individuals (GROUP_SKU_FLAG='I'), so we need to call it for every group level
            var masterSet = new Set();
            
            (groups || []).forEach(function(parentGroup){
                try {
                    // Add the parent group itself
                    masterSet.add(String(parentGroup.id));
                    
                    // Find all descendant groups under this parent using DOM traversal
                    var flatNode = flat.find(function(fn){ 
                        return String(getMasterFromNode(fn)) === String(parentGroup.id); 
                    });
                    
                    if (flatNode && tv.findByUid && flatNode.uid) {
                        var $parentLi = tv.findByUid(flatNode.uid);
                        if ($parentLi && $parentLi.length){
                            // Find all descendant .k-item elements (subgroups)
                            $parentLi.find('.k-item').each(function(){
                                var descendantItem = tv.dataItem(this);
                                if (!descendantItem) return;
                                
                                var isGroup = descendantItem && (descendantItem.hasChildren === true || (Array.isArray(descendantItem.items) && descendantItem.items.length > 0));
                                if (isGroup){
                                    var masterCode = getMasterFromNode(descendantItem);
                                    if (masterCode != null) {
                                        masterSet.add(String(masterCode));
                                    }
                                }
                            });
                        }
                    }
                } catch(e) {
                    console.warn('[DEBUG] Error processing parent group:', parentGroup, e);
                    masterSet.add(String(parentGroup.id));
                }
            });
            
            var masterCodes = Array.from(masterSet);
            console.log('[DEBUG] Master codes to fetch individuals from:', masterCodes);
            if (!masterCodes.length && leaves.length) {
                var leafRows = leaves.map(function(l){ var o = {}; o[cfg.idField] = l.id; o[cfg.codeField] = l.id; o[cfg.nameField] = l.text; return o; });
                updateGridWithData(cfg.selectedGridId, cfg, leafRows);
                return;
            }
            var reqs = (masterCodes || []).map(function (mc) {
                var url = (cfg.individualsApiUrl || '').split('?')[0] + '?groupCode=' + encodeURIComponent(mc);
                return $.ajax({ url: url, type: 'GET' });
            });
            $.when.apply($, reqs.length ? reqs : [$.Deferred().resolve([]).promise()]).done(function(){
                var args = Array.prototype.slice.call(arguments);
                var arrays = [];
                if (reqs.length === 1) {
                    var single = args[0];
                    var payload = Array.isArray(single) && single.length && single[0] && single[0].DATA ? single[0] : single;
                    var list = Array.isArray(payload) ? payload : (payload && (payload.DATA || payload.Data || payload.data)) || [];
                    arrays = [list];
                } else {
                    arrays = args.map(function (triple) {
                        var resp = triple && triple[0];
                        return Array.isArray(resp) ? resp : (resp && (resp.DATA || resp.Data || resp.data)) || [];
                    });
                }
                var raw = [].concat.apply([], arrays);
                console.log('[DEBUG] Total individuals fetched from all groups:', raw.length);
                
                // Backend only returns individuals (GROUP_SKU_FLAG='I'), so no need to filter out groups
                // Just normalize the data directly
                var data = (raw || []).map(function(r){
                    var o = {};
                    var indiv = r[cfg.idField] || r[cfg.codeField]
                        || r.CHILD_CUSTOMER_CODE || r.CHILD_PARTY_CODE || r.CHILD_SUPPLIER_CODE
                        || r.CUSTOMER_CODE || r.PARTY_TYPE_CODE || r.SUPPLIER_CODE
                        || r.ACC_CODE || r.PARTY_CODE || r.CUSTOMERID
                        || r.CODE || r.ID || r.AccCode || r.ACC_CODE;
                    var code = r[cfg.codeField]
                        || r.CHILD_CUSTOMER_CODE || r.CHILD_PARTY_CODE || r.CHILD_SUPPLIER_CODE
                        || r.CUSTOMER_CODE || r.PARTY_TYPE_CODE || r.SUPPLIER_CODE
                        || r.ACC_CODE || r.PARTY_CODE || r.CUSTOMERID
                        || r.CODE || r.AccCode || r.ACC_CODE || indiv;
                    var name = r[cfg.nameField]
                        || r.CHILD_CUSTOMER_EDESC || r.CHILD_PARTY_EDESC || r.CHILD_SUPPLIER_EDESC
                        || r.CUSTOMER_EDESC || r.PARTY_TYPE_EDESC || r.SUPPLIER_EDESC
                        || r.NAME || r.DESCRIPTION || r.EDESC || r.Edesc || code;
                    o[cfg.idField] = indiv != null ? indiv : code;
                    o[cfg.codeField] = code != null ? code : '';
                    o[cfg.nameField] = name != null ? name : '';
                    return o;
                });
                if (leaves.length > 0) {
                    var mappedLeaves = leaves.map(function (l) { var o = {}; o[cfg.idField] = l.id; o[cfg.codeField] = l.id; o[cfg.nameField] = l.text; return o; });
                    data = data.concat(mappedLeaves);
                }
                var seen = {}; var deduped = []; var idF = cfg.idField;
                (data || []).forEach(function (row) { var k = row[idF]; if (k == null || seen[k]) return; seen[k] = true; deduped.push(row); });
                try { var g = $(cfg.selectedGridId).data('kendoGrid'); if (g && g.dataSource && g.dataSource.filter) g.dataSource.filter([]); } catch(e) {}
                updateGridWithData(cfg.selectedGridId, cfg, deduped);
            }).fail(function(){ updateGridWithData(cfg.selectedGridId, cfg, []); });
        })();
        return;
    }

    function initializePartyTree(partyType, treeDataSource) {
        var cfg = partyConfigs[partyType];
        if (!cfg) return;
        var ds = treeDataSource || $(cfg.treeViewId).data('kendoTreeView')?.dataSource || new kendo.data.HierarchicalDataSource({ data: [] });
        $(cfg.treeViewId).kendoTreeView({
            dataSource: ds,
            dataTextField: 'text',
            checkboxes: { checkChildren: true, threeState: false },
            loadOnDemand: false,
            check: function () { refreshSelectedGridForParty(partyType); },
            change: function () { refreshSelectedGridForParty(partyType); }
        });
        // Apply persisted checked groups to tree
        try {
            var groupSet = getGroupSetFor(partyType);
            if (groupSet && groupSet.size > 0) {
                var tv = $(cfg.treeViewId).data('kendoTreeView');
                if (tv && tv.dataSource && typeof tv.dataSource.flatView === 'function') {
                    tv.dataSource.flatView().forEach(function(n){
                        var id = n.MASTER_CODE || n.id || n.CODE || n.groupCode || n.masterCode;
                        if (id != null && groupSet.has(String(id))) { n.set('checked', true); }
                    });
                }
            }
        } catch (e) {}
        // Ensure grid exists and reflects persisted selection
        updateGridWithData(cfg.selectedGridId, cfg, []);
    }

    function openPartyModal(partyType, opts) {
        opts = opts || {};
        var cfg = partyConfigs[partyType];
        if (!cfg) return Promise.reject('Unknown party type: ' + partyType);
        // Ensure a tree datasource: use provided, existing, or fetch from API
        var ensureDsPromise;
        var existingTv = $(cfg.treeViewId).data('kendoTreeView');
        if (opts.treeDataSource) {
            ensureDsPromise = $.Deferred().resolve(opts.treeDataSource).promise();
        } else if (existingTv && existingTv.dataSource && existingTv.dataSource.data().length > 0) {
            ensureDsPromise = $.Deferred().resolve(existingTv.dataSource).promise();
        } else {
            // Use jQuery Deferred style; handle failure via then's failure callback
            ensureDsPromise = fetchPartyHierarchyDataSource(partyType).then(null, function () {
                return new kendo.data.HierarchicalDataSource({ data: [] });
            });
        }
        return new Promise(function (resolve) {
            var $modal = $(cfg.modalId);
            // First ensure tree is initialized, then show modal
            $.when(ensureDsPromise).done(function(ds){
                if (!$(cfg.treeViewId).data('kendoTreeView')) {
                    initializePartyTree(partyType, ds);
                } else if (ds && ds.data && typeof ds.data === 'function') {
                    $(cfg.treeViewId).data('kendoTreeView').setDataSource(ds);
                }
                $modal.modal('show');
            }).fail(function(){
                // Even if fetching fails, show modal with empty tree
                if (!$(cfg.treeViewId).data('kendoTreeView')) initializePartyTree(partyType);
                $modal.modal('show');
            });
            // Ensure we initialize after the modal is fully shown (DOM sized correctly)
            $modal.off('shown.bs.modal.partyInit').on('shown.bs.modal.partyInit', function () {
                // Rebuild grid fresh so header checkbox is guaranteed visible
                var existing = $(cfg.selectedGridId).data('kendoGrid');
                if (existing) { existing.destroy(); $(cfg.selectedGridId).empty(); }
                // Do NOT clear selected set; we retain state across opens
                updateGridWithData(cfg.selectedGridId, cfg, []);
                wireGridControlsForParty(partyType);
                // Populate grid using current tree checks or persisted groups
                setTimeout(function(){ refreshSelectedGridForParty(partyType); }, 0);
                // If the user toggles Group/Individual after selecting groups, refresh the grid
                $modal.off('change.partyMode').on('change.partyMode', "input[type='radio']", function () {
                    setTimeout(function(){ refreshSelectedGridForParty(partyType); }, 0);
                });
                // Default to Group mode on open
                try {
                    var $groupRadio = $modal.find("input[type='radio'][value='group']");
                    var $indRadio = $modal.find("input[type='radio'][value='individual']");
                    if ($groupRadio.length) {
                        $groupRadio.prop('checked', true).trigger('change');
                        if ($indRadio.length) { $indRadio.prop('checked', false); }
                    }
                } catch(e) {}
                // Unbind this initializer after first run
                $modal.off('shown.bs.modal.partyInit');
            });

            $modal.off('click.partySelectConfirm').on('click.partySelectConfirm', '.btn.btn-primary', function () {
                var grid = $(cfg.selectedGridId).data('kendoGrid');
                var view = grid ? (grid.dataSource.view().toJSON ? grid.dataSource.view().toJSON() : grid.dataSource.view()) : [];
                var selectedSet = getSelectedSetFor(partyType);
                var idField = cfg.idField;
                var selectedData = Array.isArray(view) ? view.filter(function (row) { return selectedSet.has(row[idField]); }) : [];
                // If currently in Group mode, ignore row-based selection to avoid carrying over individual picks
                var isGroupMode = false;
                try {
                    isGroupMode = $modal.find("input[type='radio'][value='group']").is(':checked');
                } catch(e) { isGroupMode = false; }
                if (isGroupMode) { selectedData = []; }
                // Fallback: if user only checked groups and no rows are marked, select all visible rows
                if ((!selectedData || selectedData.length === 0) && grid) {
                    var tv = $(cfg.treeViewId).data('kendoTreeView');
                    var anyChecked = tv && tv.dataSource && typeof tv.dataSource.flatView === 'function' && tv.dataSource.flatView().some(function(n){return n && n.checked;});
                    if (anyChecked) {
                        selectedData = view && view.toJSON ? view.toJSON() : (Array.isArray(view) ? view.slice() : []);
                    }
                }
                // Also capture which master groups were checked in the tree
                var tv = $(cfg.treeViewId).data('kendoTreeView');
                var checkedMasters = [];
                if (tv && tv.dataSource && typeof tv.dataSource.flatView === 'function') {
                    tv.dataSource.flatView().forEach(function(n){
                        var isGroup = n && (n.hasChildren === true || (Array.isArray(n.items) && n.items.length > 0));
                        if (n && n.checked && isGroup) {
                            var id = n.MASTER_CODE || n.id || n.CODE || n.groupCode || n.masterCode;
                            if (id != null) checkedMasters.push(String(id));
                        }
                    });
                }
                // Persist state for next open
                var st = partyModalState[partyType] || { groups: new Set(), individuals: new Set() };
                st.groups = new Set(checkedMasters.map(String));
                st.individuals = new Set(Array.from(selectedSet));
                partyModalState[partyType] = st;
                resolve({ type: partyType, data: selectedData, checkedMasterIds: checkedMasters });
                $modal.modal('hide');
            });
        });
    }

    // Items
    function refreshSelectedGridForItems() {
        var cfg = itemConfig;
        var tv = $(cfg.treeViewId).data('kendoTreeView');
        if (!tv) { updateGridWithData(cfg.selectedGridId, cfg, []); return; }
        var nodes = getCheckedNodesInfo(tv);
        var groups = nodes.filter(function (n) { return !n.isLeaf; });
        var leaves = nodes.filter(function (n) { return n.isLeaf; });
        if (groups.length === 0 && leaves.length === 0) { updateGridWithData(cfg.selectedGridId, cfg, []); return; }
        var masterCodes = groups.map(function (g) { return g.id; });
        var fetchP = masterCodes.length > 0
            ? fetchIndividualsByMasters(cfg.genericIndividualsByMasters, masterCodes)
            : $.Deferred().resolve([]).promise();
        $.when(fetchP).done(function (resp) {
            var raw = Array.isArray(resp) ? resp : (resp.DATA || resp.Data || resp.data || []);
            // Normalize incoming item data to expected fields for filtering
            var data = (raw || []).map(function (r) {
                var o = {};
                var id = r[cfg.idField] || r.CODE || r.ID || r.ITEM_CODE;
                var code = r[cfg.codeField] || r.CODE || r.ITEM_CODE || id;
                var name = r[cfg.nameField] || r.NAME || r.DESCRIPTION || r.ITEM_EDESC || code;
                o[cfg.idField] = id != null ? id : code;
                o[cfg.codeField] = code != null ? code : '';
                o[cfg.nameField] = name != null ? name : '';
                return o;
            });
            // If batch returned nothing, fallback to per-group GET
            if ((!data || data.length === 0) && groups.length > 0) {
                var requests = groups.map(function (g) {
                    var url = (cfg.individualsApiUrl || '').split('?')[0] + '?groupCode=' + encodeURIComponent(g.id);
                    return $.ajax({ url: url, type: 'GET' });
                });
                $.when.apply($, requests.length ? requests : [$.Deferred().resolve([]).promise()]).done(function () {
                    var args = Array.prototype.slice.call(arguments);
                    var arrays = [];
                    if (requests.length === 1) {
                        var single = args[0];
                        var payload = Array.isArray(single) ? single : (single && (single.DATA || single.Data || single.data)) || [];
                        arrays = [payload];
                    } else {
                        arrays = args.map(function (triple) {
                            var resp = triple && triple[0];
                            return Array.isArray(resp) ? resp : (resp && (resp.DATA || resp.Data || resp.data)) || [];
                        });
                    }
                    var raw2 = [].concat.apply([], arrays);
                    data = (raw2 || []).map(function (r) {
                        var o = {};
                        var id = r[cfg.idField] || r.CODE || r.ID || r.ITEM_CODE;
                        var code = r[cfg.codeField] || r.CODE || r.ITEM_CODE || id;
                        var name = r[cfg.nameField] || r.NAME || r.DESCRIPTION || r.ITEM_EDESC || code;
                        o[cfg.idField] = id != null ? id : code;
                        o[cfg.codeField] = code != null ? code : '';
                        o[cfg.nameField] = name != null ? name : '';
                        return o;
                    });
                    if (leaves.length > 0) {
                        var mapped2 = leaves.map(function (l) { var o = {}; o[cfg.idField] = l.id; o[cfg.codeField] = l.id; o[cfg.nameField] = l.text; return o; });
                        data = data.concat(mapped2);
                    }
                    // De-duplicate by id
                    var seen2 = {}; var deduped2 = []; var idF2 = cfg.idField;
                    (data || []).forEach(function (row) { var k = row[idF2]; if (k == null || seen2[k]) return; seen2[k] = true; deduped2.push(row); });
                    updateGridWithData(cfg.selectedGridId, cfg, deduped2);
                }).fail(function(){ updateGridWithData(cfg.selectedGridId, cfg, []); });
                return;
            }
            if (leaves.length > 0) {
                var mapped = leaves.map(function (l) {
                    var o = {}; o[cfg.idField] = l.id; o[cfg.codeField] = l.id; o[cfg.nameField] = l.text; return o;
                });
                data = data.concat(mapped);
            }
            // De-duplicate by id
            var seen = {}; var deduped = []; var idF = cfg.idField;
            (data || []).forEach(function (row) { var k = row[idF]; if (k == null || seen[k]) return; seen[k] = true; deduped.push(row); });
            updateGridWithData(cfg.selectedGridId, cfg, deduped);
        }).fail(function () {
            // Fallback to per-group GET on failure
            if (groups.length === 0) { updateGridWithData(cfg.selectedGridId, cfg, []); return; }
            var requests = groups.map(function (g) {
                var url = (cfg.individualsApiUrl || '').split('?')[0] + '?groupCode=' + encodeURIComponent(g.id);
                return $.ajax({ url: url, type: 'GET' });
            });
            $.when.apply($, requests.length ? requests : [$.Deferred().resolve([]).promise()]).done(function () {
                var args = Array.prototype.slice.call(arguments);
                var arrays = [];
                if (requests.length === 1) {
                    var single = args[0];
                    var payload = Array.isArray(single) ? single : (single && (single.DATA || single.Data || single.data)) || [];
                    arrays = [payload];
                } else {
                    arrays = args.map(function (triple) {
                        var resp = triple && triple[0];
                        return Array.isArray(resp) ? resp : (resp && (resp.DATA || resp.Data || resp.data)) || [];
                    });
                }
                var raw = [].concat.apply([], arrays);
                var data = (raw || []).map(function (r) {
                    var o = {};
                    var id = r[cfg.idField] || r.CODE || r.ID || r.ITEM_CODE;
                    var code = r[cfg.codeField] || r.CODE || r.ITEM_CODE || id;
                    var name = r[cfg.nameField] || r.NAME || r.DESCRIPTION || r.ITEM_EDESC || code;
                    o[cfg.idField] = id != null ? id : code;
                    o[cfg.codeField] = code != null ? code : '';
                    o[cfg.nameField] = name != null ? name : '';
                    return o;
                });
                if (leaves.length > 0) {
                    var mapped = leaves.map(function (l) { var o = {}; o[cfg.idField] = l.id; o[cfg.codeField] = l.id; o[cfg.nameField] = l.text; return o; });
                    data = data.concat(mapped);
                }
                var seen = {}; var deduped = []; var idF = cfg.idField;
                (data || []).forEach(function (row) { var k = row[idF]; if (k == null || seen[k]) return; seen[k] = true; deduped.push(row); });
                updateGridWithData(cfg.selectedGridId, cfg, deduped);
            }).fail(function(){ updateGridWithData(cfg.selectedGridId, cfg, []); });
        });
    }

    function initializeItemTree(treeDataSource) {
        var cfg = itemConfig;
        var ds = treeDataSource || $(cfg.treeViewId).data('kendoTreeView')?.dataSource || new kendo.data.HierarchicalDataSource({ data: [] });
        $(cfg.treeViewId).kendoTreeView({
            dataSource: ds,
            dataTextField: 'text',
            checkboxes: { checkChildren: true, threeState: false },
            loadOnDemand: false,
            check: refreshSelectedGridForItems,
            change: refreshSelectedGridForItems
        });
        updateGridWithData(cfg.selectedGridId, cfg, []);
    }

    function openItemModal(opts) {
        opts = opts || {};
        // Ensure a tree datasource: use provided, existing, or fetch from API
        var ensureDsPromise;
        var existingTv = $(itemConfig.treeViewId).data('kendoTreeView');
        if (opts.treeDataSource) {
            ensureDsPromise = $.Deferred().resolve(opts.treeDataSource).promise();
        } else if (existingTv && existingTv.dataSource && existingTv.dataSource.data().length > 0) {
            ensureDsPromise = $.Deferred().resolve(existingTv.dataSource).promise();
        } else {
            ensureDsPromise = fetchItemHierarchyDataSource().then(null, function(){
                return new kendo.data.HierarchicalDataSource({ data: [] });
            });
        }
        return new Promise(function (resolve) {
            var $modal = $(itemConfig.modalId);
            $.when(ensureDsPromise).done(function(ds){
                if (!$(itemConfig.treeViewId).data('kendoTreeView')) {
                    initializeItemTree(ds);
                } else if (ds && ds.data && typeof ds.data === 'function') {
                    $(itemConfig.treeViewId).data('kendoTreeView').setDataSource(ds);
                }
                $modal.modal('show');
            }).fail(function(){
                if (!$(itemConfig.treeViewId).data('kendoTreeView')) initializeItemTree();
                $modal.modal('show');
            });
            // Re-init on shown to ensure header checkbox visibility and clear selection
            $modal.off('shown.bs.modal.itemInit').on('shown.bs.modal.itemInit', function () {
                var existing = $(itemConfig.selectedGridId).data('kendoGrid');
                if (existing) { existing.destroy(); $(itemConfig.selectedGridId).empty(); }
                var sel = getSelectedSetFor('item'); if (sel && sel.clear) sel.clear();
                updateGridWithData(itemConfig.selectedGridId, itemConfig, []);
                wireGridControlsForItems();
                setTimeout(function(){ refreshSelectedGridForItems(); }, 0);
                // Default to Group mode on open
                try {
                    var $groupRadio = $modal.find("input[type='radio'][value='group']");
                    var $indRadio = $modal.find("input[type='radio'][value='individual']");
                    if ($groupRadio.length) {
                        $groupRadio.prop('checked', true).trigger('change');
                        if ($indRadio.length) { $indRadio.prop('checked', false); }
                    }
                } catch(e) {}
                $modal.off('shown.bs.modal.itemInit');
            });
            $modal.off('click.itemSelectConfirm').on('click.itemSelectConfirm', '.btn.btn-primary', function () {
                var grid = $(itemConfig.selectedGridId).data('kendoGrid');
                var data = grid ? grid.dataSource.view().toJSON ? grid.dataSource.view().toJSON() : grid.dataSource.view() : [];
                // Only return rows whose id is checked in selection set
                var selectedSet = getSelectedSetFor('item');
                var idField = itemConfig.idField;
                var selectedData = Array.isArray(data) ? data.filter(function (r) { return selectedSet.has(r[idField]); }) : [];
                // If currently in Group mode, ignore row-based selection to avoid carrying over individual picks
                var isGroupMode = false;
                try {
                    isGroupMode = $modal.find("input[type='radio'][value='group']").is(':checked');
                } catch(e) { isGroupMode = false; }
                if (isGroupMode) { selectedData = []; }
                // Fallback: if user only checked groups and no rows are marked, select all visible rows
                if ((!selectedData || selectedData.length === 0) && grid) {
                    var tv = $(itemConfig.treeViewId).data('kendoTreeView');
                    var anyChecked = tv && tv.dataSource && typeof tv.dataSource.flatView === 'function' && tv.dataSource.flatView().some(function(n){return n.checked;});
                    if (anyChecked) {
                        selectedData = data && data.toJSON ? data.toJSON() : (Array.isArray(data) ? data.slice() : []);
                    }
                }
                // Capture which item master groups were checked in the tree
                var tvChecked = $(itemConfig.treeViewId).data('kendoTreeView');
                var checkedMasters = [];
                if (tvChecked && tvChecked.dataSource && typeof tvChecked.dataSource.flatView === 'function') {
                    tvChecked.dataSource.flatView().forEach(function(n){
                        var isGroup = n && (n.hasChildren === true || (Array.isArray(n.items) && n.items.length > 0));
                        if (n && n.checked && isGroup) {
                            var id = n.MASTER_CODE || n.id || n.CODE || n.groupCode || n.masterCode;
                            if (id != null) checkedMasters.push(String(id));
                        }
                    });
                }
                resolve({ type: 'item', data: selectedData, checkedMasterIds: checkedMasters });
                $modal.modal('hide');
            });
        });
    }

    global.PartyItemSelector = {
        openPartyModal: openPartyModal,
        openItemModal: openItemModal,
        initializePartyTree: initializePartyTree,
        initializeItemTree: initializeItemTree,
        refreshSelectedGridForParty: refreshSelectedGridForParty,
        refreshSelectedGridForItems: refreshSelectedGridForItems
    };

})(window, jQuery);
