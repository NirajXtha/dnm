var wijmo;
(function (wijmo) {
    (function (grid) {
        'use strict';

        /**
        * ExcelConverter class provides function exporting FlexGrid to xlsx file
        * and importing xlsx file to FlexGrid.
        */
        var ExcelConverter = (function () {
            function ExcelConverter() {
            }
            /**
            * export the FlexGrid to xlsx file
            *
            * @param flex the FlexGrid need be exported to xlsx file
            * @param includeColumnHeader indicates whether export the column header
            */
            ExcelConverter.export = function (flex, convertOption) {
                if (typeof convertOption === "undefined") { convertOption = { includeColumnHeader: true }; }
                var file = {
                    worksheets: [],
                    creator: 'Mike Lu',
                    created: new Date('11/4/2014'),
                    lastModifiedBy: 'Mike Lu',
                    modified: new Date(),
                    activeWorksheet: 0
                }, worksheetData = [], columnSettings = [], workSheet = {
                    data: [],
                    frozenPane: {}
                }, groupLevel = 0, worksheetDataHeader, rowHeight, column, row, groupRow, isGroupRow, value, columnSetting, result;

                // add the headers in the worksheet.
                if (convertOption.includeColumnHeader) {
                    for (var ri = 0; ri < flex.columnHeaders.rows.length; ri++) {
                        worksheetDataHeader = [];
                        rowHeight = flex.columnHeaders.rows[ri].height;
                        if (rowHeight) {
                            rowHeight = rowHeight * 72 / 96;
                        }
                        for (var ci = 0; ci < flex.columnHeaders.columns.length; ci++) {
                            column = flex.columnHeaders.columns[ci];
                            value = flex.columnHeaders.getCellData(ri, ci, false);

                            if (ri === 0) {
                                columnSetting = this._getColumnSetting(column, flex.columnHeaders.columns.defaultSize);
                                columnSettings.push(columnSetting);
                            }

                            worksheetDataHeader.push({
                                value: value,
                                bold: true,
                                autoWidth: true,
                                hAlign: columnSetting.alignment,
                                width: columnSetting.width,
                                height: rowHeight,
                                visible: columnSetting.visible
                            });
                        }
                        if (worksheetDataHeader.length > 0) {
                            worksheetDataHeader[0].rowVisible = true;
                        }
                        worksheetData.push(worksheetDataHeader);
                    }
                } else {
                    for (var ci = 0; ci < flex.columnHeaders.columns.length; ci++) {
                        column = flex.columnHeaders.columns[ci];

                        columnSetting = this._getColumnSetting(column, flex.columnHeaders.columns.defaultSize);
                        columnSettings.push(columnSetting);
                    }
                }

                for (ri = 0; ri < flex.cells.rows.length; ri++) {
                    row = flex.rows[ri];
                    isGroupRow = row instanceof grid.GroupRow;

                    if (isGroupRow) {
                        groupRow = wijmo.tryCast(row, grid.GroupRow);
                        groupLevel = groupRow.level + 1;
                    }

                    // Only the common grid row and group row need be exported to xlsx file.
                    if (row.constructor === wijmo.grid.Row || isGroupRow) {
                        worksheetData.push(this._parseFlexGridRowToSheetRow(flex, row, ri, columnSettings, isGroupRow, groupLevel));
                    }
                }

                workSheet.data = worksheetData;
                workSheet.frozenPane = {
                    rows: convertOption.includeColumnHeader ? (flex.frozenRows + flex.columnHeaders.rows.length) : flex.frozenRows,
                    columns: flex.frozenColumns
                };

                file.worksheets.push(workSheet);

                result = xlsx(file);

                result.base64Array = this._base64DecToArr(result.base64);

                return result;
            };

            /**
            * import the xlsx file
            *
            * @param file the base64 string converted from xlsx file
            * @param flex the Flex Grid need bind the data import from xlsx file
            * @param includeColumnHeader indicates whether imported the column header for the FlexGrid
            * @param callback provides an callback function after finishing importing excel to flex grid.
            */
            ExcelConverter.import = function (file, flex, convertOption) {
                if (typeof convertOption === "undefined") { convertOption = { includeColumnHeader: true }; }
                var fileData = this._base64EncArr(new Uint8Array(file)), fileObj = xlsx(fileData), currentIncludeRowHeader = convertOption.includeColumnHeader, c = 0, r = 0, columns = [], columnSetting, column, columnHeader, sheetHeaders, sheetHeader, headerForamt, row, currentSheet, columnCount, isGroupHeader, item, nextRowIdx, nextRow, isGroupBelow, groupRow, frozenColumns, frozenRows;

                flex.columns.clear();
                flex.rows.clear();
                flex.frozenColumns = 0;
                flex.frozenRows = 0;

                if (fileObj.worksheets.length === 0) {
                    return;
                }

                currentSheet = fileObj.worksheets[0];
                if (convertOption.includeColumnHeader) {
                    r = 1;
                    if (currentSheet.data.length <= 1) {
                        currentIncludeRowHeader = false;
                        r = 0;
                    }
                    sheetHeaders = currentSheet.data[0];
                }
                columnCount = this._getColumnCount(currentSheet.data);
                isGroupBelow = currentSheet.isGroupBelow;

                for (; c < columnCount; c++) {
                    flex.columns.push(new wijmo.grid.Column());
                }

                for (; r < currentSheet.data.length; r++) {
                    isGroupHeader = false;
                    row = currentSheet.data[r];

                    if (row && row[0]) {
                        nextRowIdx = r + 1;
                        while (nextRowIdx < currentSheet.data.length) {
                            nextRow = currentSheet.data[nextRowIdx];
                            if (nextRow) {
                                if ((isNaN(row[0].groupLevel) && !isNaN(nextRow[0].groupLevel)) || (!isNaN(row[0].groupLevel) && row[0].groupLevel < nextRow[0].groupLevel)) {
                                    isGroupHeader = true;
                                }
                                break;
                            } else {
                                nextRowIdx++;
                            }
                        }
                    }

                    if (isGroupHeader && isGroupBelow) {
                        groupRow = new grid.GroupRow();
                        groupRow.level = isNaN(row[0].groupLevel) ? 0 : row[0].groupLevel;
                        flex.rows.push(groupRow);
                    } else {
                        flex.rows.push(new grid.Row());
                    }

                    for (c = 0; c < columnCount; c++) {
                        if (!row) {
                            flex.setCellData(currentIncludeRowHeader ? r - 1 : r, c, '');
                            this._setColumn(columns, c, undefined);
                        } else {
                            item = row[c];
                            flex.setCellData(currentIncludeRowHeader ? r - 1 : r, c, this._getItemValue(item));
                            if (!isGroupHeader) {
                                this._setColumn(columns, c, item);
                            }
                            if (item && !item.visible && columns[c]) {
                                columns[c].visible = false;
                            }
                        }
                    }

                    if (row && row[0] && !row[0].rowVisible) {
                        flex.rows[currentIncludeRowHeader ? r - 1 : r].visible = false;
                    }
                }

                if (currentSheet.frozenPane) {
                    frozenColumns = currentSheet.frozenPane.columns;
                    if (wijmo.isNumber(frozenColumns) && !isNaN(frozenColumns)) {
                        flex.frozenColumns = frozenColumns;
                    }

                    frozenRows = currentSheet.frozenPane.rows;
                    if (wijmo.isNumber(frozenRows) && !isNaN(frozenRows)) {
                        flex.frozenRows = currentIncludeRowHeader && frozenRows > 0 ? frozenRows - 1 : frozenRows;
                    }
                }

                for (c = 0; c < flex.columnHeaders.columns.length; c++) {
                    columnSetting = columns[c];
                    column = flex.columns[c];
                    if (currentIncludeRowHeader) {
                        sheetHeader = sheetHeaders ? sheetHeaders[c] : undefined;
                        if (sheetHeader && sheetHeader.value) {
                            headerForamt = this._parseExcelFormat(sheetHeader);
                            columnHeader = wijmo.Globalize.format(sheetHeader.value, headerForamt);
                        } else {
                            columnHeader = this._numAlpha(c);
                        }
                    } else {
                        columnHeader = this._numAlpha(c);
                    }
                    column.header = columnHeader;
                    column.dataType = columnSetting.dataType;
                    column.format = columnSetting.format;
                    column.visible = columnSetting.visible;
                }
            };

            // Parse the row data of flex grid to a sheet row
            ExcelConverter._parseFlexGridRowToSheetRow = function (flex, row, rowIndex, columnSettings, isGroupRow, groupLevel) {
                var rowHeight = row.height, worksheetDataItem = [], groupName = undefined, colSpan = 0, groupNameAdded = false, columnSetting, format, val;

                if (rowHeight) {
                    rowHeight = rowHeight * 72 / 96;
                }

                for (var ci = 0; ci < flex.columnHeaders.columns.length; ci++) {
                    columnSetting = columnSettings[ci];
                    val = flex.getCellData(rowIndex, ci, false);
                    format = columnSetting.format ? this._parseCellFormat(columnSetting.format) : wijmo.isDate(val) ? this._formatMap['d'] : !wijmo.isNumber(val) || wijmo.isInt(val) ? 'General' : this._formatMap['n'];

                    if (isGroupRow) {
                        // Process the group row of the flex grid.
                        if (row.dataItem) {
                            if (val) {
                                // Add the group header in the non-aggregate fields.
                                if (groupName && !groupNameAdded) {
                                    worksheetDataItem.push({
                                        value: row.dataItem ? row.dataItem.groupDescription.propertyName + ': ' + groupName + ' (' + row.dataItem.items.length + ' items)' : groupName,
                                        colSpan: colSpan,
                                        bold: true,
                                        autoWidth: true,
                                        height: rowHeight,
                                        visible: true,
                                        groupLevel: groupLevel - 1,
                                        indent: groupLevel - 1
                                    });
                                    groupNameAdded = true;
                                }
                                worksheetDataItem.push({
                                    value: val,
                                    formatCode: format,
                                    bold: true,
                                    autoWidth: true,
                                    hAlign: wijmo.isDate(val) && columnSetting.alignment === '' ? 'left' : columnSetting.alignment,
                                    width: columnSetting.width,
                                    height: rowHeight,
                                    visible: columnSetting.visible,
                                    groupLevel: groupLevel - 1
                                });
                            } else {
                                groupName = groupName || row.dataItem ? row.dataItem.name : val;
                                colSpan++;
                                if (ci === flex.columnHeaders.columns.length - 1 && !groupNameAdded) {
                                    worksheetDataItem.push({
                                        value: row.dataItem ? row.dataItem.groupDescription.propertyName + ': ' + groupName + ' (' + row.dataItem.items.length + ' items)' : groupName,
                                        colSpan: colSpan,
                                        bold: true,
                                        autoWidth: true,
                                        height: rowHeight,
                                        visible: true,
                                        groupLevel: groupLevel - 1,
                                        indent: groupLevel - 1
                                    });
                                }
                            }
                        } else {
                            if (!groupNameAdded) {
                                worksheetDataItem.push({
                                    value: val,
                                    bold: true,
                                    autoWidth: true,
                                    height: rowHeight,
                                    width: columnSetting.width,
                                    visible: true,
                                    groupLevel: groupLevel - 1,
                                    indent: groupLevel - 1
                                });
                                groupNameAdded = true;
                            } else {
                                worksheetDataItem.push({
                                    value: val,
                                    formatCode: format,
                                    bold: true,
                                    autoWidth: true,
                                    hAlign: wijmo.isDate(val) && columnSetting.alignment === '' ? 'left' : columnSetting.alignment,
                                    width: columnSetting.width,
                                    height: rowHeight,
                                    visible: columnSetting.visible,
                                    groupLevel: groupLevel - 1
                                });
                            }
                        }
                    } else {
                        // Add the cell content
                        worksheetDataItem.push({
                            value: val,
                            formatCode: format,
                            autoWidth: true,
                            hAlign: wijmo.isDate(val) && columnSetting.alignment === '' ? 'left' : columnSetting.alignment,
                            width: columnSetting.width,
                            height: rowHeight,
                            visible: columnSetting.visible,
                            groupLevel: groupLevel
                        });
                    }
                }

                if (worksheetDataItem.length > 0) {
                    worksheetDataItem[0].rowVisible = row.visible;
                }

                return worksheetDataItem;
            };

            // Parse the cell format of flex grid to excel format.
            ExcelConverter._parseCellFormat = function (format) {
                var dec = 0, mapFormat = this._formatMap[format[0]], decimalArray = [], xlsxFormat;

                if (format.length > 1) {
                    dec = parseInt(format.substr(1));
                }

                if (!isNaN(dec)) {
                    for (var i = 0; i < dec; i++) {
                        decimalArray.push(0);
                    }
                }

                if (decimalArray.length > 0) {
                    xlsxFormat = mapFormat.replace(/\.00/g, '.' + decimalArray.join(''));
                } else {
                    if (mapFormat) {
                        xlsxFormat = mapFormat;
                    } else {
                        xlsxFormat = format.replace(/tt/, 'AM/PM');
                    }
                }

                return xlsxFormat;
            };

            // parse the basic excel format to js format
            ExcelConverter._parseExcelFormat = function (item) {
                if (item === undefined || item === null || item.value === undefined || item.value === null || isNaN(item.value)) {
                    return undefined;
                }

                if (!item.formatCode || item.formatCode === 'General') {
                    return '';
                }

                var format = '', lastDotIndex;
                if (wijmo.isDate(item.value)) {
                    format = item.formatCode.replace(/[\/\s\-,]m+|m+[\/\s\-,]/, function (str) {
                        return str.toUpperCase();
                    }).replace(/\\[\-\s,]/g, function (str) {
                        return str.substring(1);
                    }).replace(/;@/g, '').replace(/\[\$\-.+\]/g, '');
                } else if (wijmo.isNumber(item.value)) {
                    lastDotIndex = item.formatCode.lastIndexOf('.');
                    if (item.formatCode.indexOf('$') > -1) {
                        format = 'c';
                    } else if (item.formatCode[item.formatCode.length - 1] === '%') {
                        format = 'p';
                    } else {
                        format = 'n';
                    }

                    if (lastDotIndex > -1) {
                        format += item.formatCode.substring(lastDotIndex, item.formatCode.lastIndexOf('0')).length;
                    } else {
                        format += '0';
                    }
                }

                return format;
            };

            // Gets the column setting, include width, visible, format and alignment
            ExcelConverter._getColumnSetting = function (column, defaultWidth) {
                var width = column.width;

                width = width ? width / 8 : defaultWidth / 8;

                return {
                    width: width,
                    visible: column.visible,
                    format: column.format,
                    alignment: column.getAlignment()
                };
            };

            // gets column count for specific row
            ExcelConverter._getColumnCount = function (sheetData) {
                var columnCount = 0, data;

                for (var i = 0; i < sheetData.length; i++) {
                    data = sheetData[i];
                    if (data && data.length > columnCount) {
                        columnCount = data.length;
                    }
                }

                return columnCount;
            };

            // convert the column index to alphabet
            ExcelConverter._numAlpha = function (i) {
                var t = Math.floor(i / 26) - 1;
                return (t > -1 ? this._numAlpha(t) : '') + this._alphabet.charAt(i % 26);
            };

            // Get DataType for value of the specific excel item
            ExcelConverter._getItemType = function (item) {
                if (item === undefined || item === null || item.value === undefined || item.value === null || isNaN(item.value)) {
                    return undefined;
                }

                return wijmo.getType(item.value);
            };

            // Set column definition for the Flex Grid
            ExcelConverter._setColumn = function (columns, columnIndex, item) {
                var dataType;
                if (!columns[columnIndex]) {
                    columns.push({
                        visible: true,
                        dataType: this._getItemType(item),
                        format: this._parseExcelFormat(item)
                    });
                } else {
                    dataType = this._getItemType(item);
                    if (columns[columnIndex].dataType === undefined || (dataType !== undefined && dataType !== 1 /* String */ && columns[columnIndex].dataType === 1 /* String */)) {
                        columns[columnIndex].dataType = dataType;
                    }

                    if (!columns[columnIndex].format) {
                        columns[columnIndex].format = this._parseExcelFormat(item);
                    }
                }
            };

            // Get value from the excel cell item
            ExcelConverter._getItemValue = function (item) {
                if (item === undefined || item === null || item.value === undefined || item.value === null) {
                    return undefined;
                }

                var val = item.value;

                if (wijmo.isNumber(val) && isNaN(val)) {
                    return '';
                } else if (val instanceof Date && isNaN(val.getTime())) {
                    return '';
                } else {
                    return val;
                }
            };

            // taken from: https://developer.mozilla.org/en-US/docs/Web/JavaScript/Base64_encoding_and_decoding#The_.22Unicode_Problem.22
            ExcelConverter._b64ToUint6 = function (nChr) {
                return nChr > 64 && nChr < 91 ? nChr - 65 : nChr > 96 && nChr < 123 ? nChr - 71 : nChr > 47 && nChr < 58 ? nChr + 4 : nChr === 43 ? 62 : nChr === 47 ? 63 : 0;
            };

            // decode the base64 string to int array
            ExcelConverter._base64DecToArr = function (sBase64, nBlocksSize) {
                var sB64Enc = sBase64.replace(/[^A-Za-z0-9\+\/]/g, ""), nInLen = sB64Enc.length, nOutLen = nBlocksSize ? Math.ceil((nInLen * 3 + 1 >> 2) / nBlocksSize) * nBlocksSize : nInLen * 3 + 1 >> 2, taBytes = new Uint8Array(nOutLen);

                for (var nMod3, nMod4, nUint24 = 0, nOutIdx = 0, nInIdx = 0; nInIdx < nInLen; nInIdx++) {
                    nMod4 = nInIdx & 3;
                    nUint24 |= this._b64ToUint6(sB64Enc.charCodeAt(nInIdx)) << 18 - 6 * nMod4;
                    if (nMod4 === 3 || nInLen - nInIdx === 1) {
                        for (nMod3 = 0; nMod3 < 3 && nOutIdx < nOutLen; nMod3++, nOutIdx++) {
                            taBytes[nOutIdx] = nUint24 >>> (16 >>> nMod3 & 24) & 255;
                        }
                        nUint24 = 0;
                    }
                }
                return taBytes;
            };

            // taken from https://developer.mozilla.org/en-US/docs/Web/API/WindowBase64/Base64_encoding_and_decoding
            /* Base64 string to array encoding */
            ExcelConverter._uint6ToB64 = function (nUint6) {
                return nUint6 < 26 ? nUint6 + 65 : nUint6 < 52 ? nUint6 + 71 : nUint6 < 62 ? nUint6 - 4 : nUint6 === 62 ? 43 : nUint6 === 63 ? 47 : 65;
            };

            ExcelConverter._base64EncArr = function (aBytes) {
                var nMod3 = 2, sB64Enc = "";

                for (var nLen = aBytes.length, nUint24 = 0, nIdx = 0; nIdx < nLen; nIdx++) {
                    nMod3 = nIdx % 3;
                    if (nIdx > 0 && (nIdx * 4 / 3) % 76 === 0) {
                        sB64Enc += "\r\n";
                    }
                    nUint24 |= aBytes[nIdx] << (16 >>> nMod3 & 24);
                    if (nMod3 === 2 || aBytes.length - nIdx === 1) {
                        sB64Enc += String.fromCharCode(this._uint6ToB64(nUint24 >>> 18 & 63), this._uint6ToB64(nUint24 >>> 12 & 63), this._uint6ToB64(nUint24 >>> 6 & 63), this._uint6ToB64(nUint24 & 63));
                        nUint24 = 0;
                    }
                }

                return sB64Enc.substr(0, sB64Enc.length - 2 + nMod3) + (nMod3 === 2 ? '' : nMod3 === 1 ? '=' : '==');
            };
            ExcelConverter._formatMap = {
                n: '#,##0.00',
                c: '$#,##0.00_);($#,##0.00)',
                p: '0.00%',
                d: 'm/dd/yyyy'
            };
            ExcelConverter._alphabet = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ';
            return ExcelConverter;
        })();
        grid.ExcelConverter = ExcelConverter;
    })(wijmo.grid || (wijmo.grid = {}));
    var grid = wijmo.grid;
})(wijmo || (wijmo = {}));



//# sourceMappingURL=ExcelConverter.js.map
