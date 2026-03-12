(function () {
    'use strict';

    // Define the Angular module
    angular.module('aiAssistantApp', [])
        .controller('aiAssistantCtrl', ['$scope', '$http', '$timeout', '$sce', aiAssistantCtrl]);

    function aiAssistantCtrl($scope, $http, $timeout, $sce) {
        var vm = this;


        vm.dummyMonthlySalesReport = `{
                              "status": "success",
                              "session_id": null,
                              "result": {
                                "data": {
                                  "MONTH": ["Shrawan", "Bhadra", "Ashoj", "Kartik", "Mangsir", "Poush", "Magh", "Falgun", "Chaitra", "Baishakh", "Jestha"],
                                  "QTY": ["452955.73", "359319.26", "266775.88", "334789.69", "235604.15", "181642.08", "418000.27", "365736.66", "378602.35", "363590.8", "51303.45"],
                                  "NET SALES": ["48970428.42", "40550519.02", "29377532.82", "33174863.09", "25338742", "16938092.38", "41944139.83", "38107737.76", "38238005.13", "37740022.95", "3842620.06"]
                                },
                                "query": "SELECT MONTH, SUM(item_wise_qty) AS \\"QTY\\", SUM(net_sales) AS \\"NET SALES\\" FROM (SELECT DISTINCT invoice_no, net_sales , SUM(item_wise_qty) AS item_wise_qty,MONTH,STARTDATE FROM AI_TEST_56 where company_code = 01 GROUP BY MONTH,STARTDATE,invoice_no, net_sales) GROUP BY MONTH,STARTDATE ORDER BY STARTDATE",
                                "question": "month wise sales report",
                                "graph_keys": [["MONTH", "QTY"], ["MONTH", "NET SALES"]],
                                "desc": "Below is the response we've prepared for you."
                              }
                            }`;

        vm.isCalledDummy = false;



        //  vm.aiAPIURL = "http://192.168.200.118:8848";
        vm.tempToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJmcmVzaCI6ZmFsc2UsImlhdCI6MTc3MDA5MzU1MiwianRpIjoiYTgwZWJlNTctNGU1Zi00NTRkLWI0NjctYmUzODgyN2Q5ZjkwIiwidHlwZSI6ImFjY2VzcyIsInN1YiI6eyJ1c2VyX25vIjoiMiIsImRiX3VzZXIiOiJBUEw4MTgyIiwiZGJfcGFzcyI6IkFQTDgxODIifSwibmJmIjoxNzcwMDkzNTUyLCJjc3JmIjoiNWE1NzAwNTktYzYzZC00MGUxLTk0ZTEtMTI5OTU1MWI1NzJkIiwiZXhwIjoxODAxNjI5NTUyfQ.IytV2KXHyIc9uUBE8US2NO0sDIeyT8eh_vTmq1AYV6w";


        vm.aiAPIURL = "http://202.79.34.39:3090";

        /// alert(vm.databaseName);
        // Message history
        vm.messages = [
            {
                type: 'assistant',
                text: 'Hello! neo-assistant here, your digital sidekick, at your service!',
                isTable: false
            }
        ];

        // User input
        vm.userPrompt = "";
        vm.isLoading = false;
        vm.authToken = "";
        vm.showPromptLibrary = false;

        // Prompt Library Keywords
        vm.promptLibrary = [
            //{ text: "Create an image", icon: "🎨" },
            //{ text: "Recommend a product", icon: "🛍️" },
            //{ text: "Improve writing", icon: "✍️" },
            //{ text: "Take a quiz", icon: "📝" },
            { text: "Monthly sales report", icon: "📄" },
            // { text: "gross sales of aclofen for party ram", icon: "📄" },
            { text: "weekly sales of report", icon: "📄" },
            { text: "daily sales report", icon: "📄" },
            { text: "item wise sales report", icon: "📄" }

            //{ text: "Learn a new skill", icon: "🎓" },
            //{ text: "Find the best deal", icon: "💰" },
            //{ text: "Fix a clunky sentence", icon: "🔧" }
        ];

        // Select a prompt from the library
        vm.selectPrompt = function (prompt) {
            vm.userPrompt = prompt.text;
            vm.showPromptLibrary = false;
            // Focus on input field if possible
            document.getElementById('aiAssistantPromptInput').focus();
        };

        // Initialization
        vm.init = function () {
            vm.getAuthToken();
        };


        // Fetch Bearer Token from API
        vm.getAuthToken = function () {
            var url = vm.aiAPIURL + '/get_token'; // Assuming /login endpoint
            var loginData = {
                //"db_user": "APL8182" // vm.databaseName  // this db name replace with real one
                // "database": vm.databaseName
                "db_user": vm.databaseName
            };

            $http.post(url, loginData).then(function (response) {
                if (response && response.data && response.data.data) {
                    vm.authToken = response.data.data;
                    console.log("Token fetched successfully");


                    //if (!vm.authToken) {
                    //    vm.authToken = vm.tempToken;
                    //}


                } else {

                    if (!vm.authToken) {
                        vm.authToken = vm.tempToken;
                    }

                    if (response && response.msg) {
                        // alert(response.msg);
                        return;
                    }
                }
            }, function (error) {
                if (error && error.data && error.data.msg) {
                    // alert(error.data.msg);
                }
                console.error("Failed to fetch auth token:", error);


                if (!vm.authToken) {
                    vm.authToken = vm.tempToken; //"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJmcmVzaCI6ZmFsc2UsImlhdCI6MTc3MDAzMzE5NywianRpIjoiYWNmYjE2NjYtNDVkOC00NTM3LTg0NTctYWE4YmUzNzI4Nzg2IiwidHlwZSI6ImFjY2VzcyIsInN1YiI6eyJ1c2VyX25vIjoiMiIsImRiX3VzZXIiOiJBUEw4MTgyIiwiZGJfcGFzcyI6IkFQTDgxODIifSwibmJmIjoxNzcwMDMzMTk3LCJjc3JmIjoiMGZmNTI2MTYtMTJkMC00YmI2LTg5ODUtODBlN2RlMDk4ZGVmIiwiZXhwIjoxODAxNTY5MTk3fQ.EDQS93k1ShRqSqgkk8q7kFR2CSbp3ldVhES98tfgMdQ";
                }
            });

        };

        // API function to fetch response from Synergy AI
        vm.getAIResponse = function (prompt, sessionId, additionalData) {
            var url = vm.aiAPIURL + '/syn_assistant';
            var data = {
                "text": prompt,
                "company_code": vm.companyCode, // Use dynamic company code
            };
            if (sessionId) {
                data.session_id = sessionId;
            }
            if (additionalData) {
                angular.extend(data, additionalData);
            }
            var config = {
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': 'Bearer ' + vm.authToken
                    //'Authorization': 'Bearer ' + "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJmcmVzaCI6ZmFsc2UsImlhdCI6MTc2OTY4MTY5NywianRpIjoiYTAwZjc2ZjYtMDkyMS00NDRhLTg3NzMtZjNhYjViNWYxMmMyIiwidHlwZSI6ImFjY2VzcyIsInN1YiI6IkFETUlOIiwibmJmIjoxNzY5NjgxNjk3LCJjc3JmIjoiOTk0OTQxOTQtMjk4ZS00ZDBlLTkxOGYtZDE3NzhmZDlmN2I4IiwiZXhwIjoxODAxMjE3Njk3LCJkYl91c2VyIjoiQVBMODE4MiIsImRiX3Bhc3MiOiJBUEw4MTgyIn0.GmFSqW5X4qvF5lR9REegPvyrFZCnMoRGU5pFeTO6v-s"
                }
            };
            return $http.post(url, data, config);
        };

        // Send message function
        vm.sendMessage = function () {

            if (!vm.authToken) { alert('Unable to connect on AI services'); return; }

            if (!vm.userPrompt || vm.userPrompt.trim() === "") return;

            var userMsgText = vm.userPrompt;
            vm.userPrompt = ""; // Clear input

            // 1. Add User Message
            vm.messages.push({
                type: 'user',
                text: userMsgText,
                isTable: false
            });

            vm.isLoading = true;

            // 2. Add "Fetching data..." message
            var loadingMsg = {
                type: 'assistant',
                text: 'Fetching ' + userMsgText + ' data...',
                isTable: false,
                isLoading: true
            };
            vm.messages.push(loadingMsg);

            // Call the real API
            vm.getAIResponse(userMsgText).then(function (response) {

                console.log(response);
                vm.isLoading = false;
                loadingMsg.isLoading = false; // Stop animation
                // Extract data from response

                // remove ''' from start and end
                response = response.data.replace(/^'''|'''$/g, '');
                // convert to JSON object
                var jsonObj = JSON.parse(response);
                var apiData = jsonObj;


                // Add success message (use 'desc' from API if available, otherwise default)
                var assistantText = (apiData.result && apiData.result.desc)
                    || apiData.desc
                    || 'Your detailed list is prepared and ready for you.';

                vm.messages.push({
                    type: 'assistant',
                    text: assistantText,
                    isTable: false
                });

                debugger;
                // Check if result data exists for table rendering
                if (apiData.status === "success" && apiData.result && apiData.result.data) {
                    // Format Table
                    var tableData = vm.formatTableData(apiData.result.data);

                    // Add Table Message
                    vm.messages.push({
                        type: 'assistant',
                        text: '',
                        isTable: true,
                        table: tableData,
                        isCollapsed: false,
                        vizType: 'table',
                        showMenu: false,
                        rawData: apiData.result.data,
                        graphKeys: apiData.result.graph_keys,
                        selectedKeyIndex: 0,
                        showKeySelector: false
                    });
                } else if (apiData.status === "multiple_matches") {
                    // Show clarification options
                    vm.messages.push({
                        type: 'assistant',
                        text: 'Multiple records found. Please select from the following options:',
                        isOptions: true,
                        options: apiData.options,
                        sessionId: apiData.session_id,
                        originalText: apiData.text,
                        selections: {} // store user choices { "item": { code: "...", name: "..." }, "party": ... }
                    });
                } else if (apiData.status !== "success") {
                    vm.messages.push({
                        type: 'assistant',
                        text: 'Sorry, something went wrong with the AI service.',
                        isTable: false
                    });
                }
                vm.scrollToBottom();

            }, function (error) {

                //DummyData
                vm.prepareWithDummyData();

                setTimeout(function () {
                    vm.isLoading = false;
                    loadingMsg.isLoading = false;

                    if (vm.isCalledDummy == false) {
                        vm.messages.push({
                            type: 'assistant',
                            text: 'Error: Failed to connect to the AI service. Please check your connection.',
                            isTable: false
                        });
                    }
                    console.error("API Error: ", error);
                    vm.scrollToBottom();
                }, 1000);

            });
        };


        vm.prepareWithDummyData = function () {

            var jsonObj = JSON.parse(vm.dummyMonthlySalesReport);
            var apiData = jsonObj;


            // Add success message (use 'desc' from API if available, otherwise default)
            var assistantText = (apiData.result && apiData.result.desc)
                || apiData.desc
                || 'Your detailed list is prepared and ready for you.';

            vm.messages.push({
                type: 'assistant',
                text: assistantText,
                isTable: false
            });

            var tableData = vm.formatTableData(apiData.result.data);

            // Add Table Message
            vm.messages.push({
                type: 'assistant',
                text: '',
                isTable: true,
                table: tableData,
                isCollapsed: false,
                vizType: 'table',
                showMenu: false,
                rawData: apiData.result.data,
                graphKeys: apiData.result.graph_keys,
                selectedKeyIndex: 0,
                showKeySelector: false
            });

            vm.isCalledDummy = true;
        };

        // Format raw JSON data into table structure
        vm.formatTableData = function (data) {

            console.log(data);

            var headers = Object.keys(data);
            var rows = [];

            // Assume all columns have same number of rows
            var rowCount = data[headers[0]].length;

            for (var i = 0; i < rowCount; i++) {
                var row = {};
                headers.forEach(function (header) {
                    row[header] = data[header][i];
                });
                rows.push(row);
            }

            return {
                headers: headers,
                rows: rows
            };
        };

        // Toggle table collapse state
        vm.toggleTable = function (msg) {
            msg.isCollapsed = !msg.isCollapsed;
        };

        // Scroll to bottom helper
        vm.scrollToBottom = function () {
            $timeout(function () {
                var chatWindow = document.querySelector('.chat-history');
                if (chatWindow) {
                    chatWindow.scrollTop = chatWindow.scrollHeight;
                }
            }, 100);
        };

        // Submenu toggle (from original code)
        vm.toggleSubmenu = function (event, submenuId) {
            var submenu = document.getElementById(submenuId);
            var arrow = event.currentTarget.querySelector('.submenu-arrow');

            if (submenu) {
                submenu.classList.toggle('expanded');
            }
            if (arrow) {
                arrow.classList.toggle('expanded');
            }
        };

        // Select an option from multiple matches
        vm.selectOption = function (msg, category, option) {
            msg.selections[category] = option;
        };

        // Check if all categories in a message have a selection
        vm.allOptionsSelected = function (msg) {
            if (!msg.isOptions || !msg.options) return false;
            var categories = Object.keys(msg.options);
            return categories.every(function (cat) {
                return msg.selections[cat] !== undefined;
            });
        };

        // Confirm options and send back to API
        vm.confirmOptions = function (msg) {
            var categories = Object.keys(msg.selections);
            var choiceSummary = categories.map(function (cat) {
                return cat + ": " + msg.selections[cat].name;
            }).join(", ");

            // Show user's confirmed choice in chat
            vm.messages.push({
                type: 'user',
                text: "My selection: " + choiceSummary,
                isTable: false
            });

            vm.isLoading = true;
            var processingMsg = {
                type: 'assistant',
                text: 'Processing your request with selected options...',
                isTable: false,
                isLoading: true
            };
            vm.messages.push(processingMsg);

            // Construct choices payload (e.g., item_choice, party_choice)
            var additionalData = {};
            categories.forEach(function (cat) {
                additionalData[cat + "_choice"] = msg.selections[cat].name;
            });

            // Ensure all original categories are present as _choice keys
            Object.keys(msg.options).forEach(function (cat) {
                if (!additionalData[cat + "_choice"]) {
                    additionalData[cat + "_choice"] = "";
                }
            });

            // Construct reinforced prompt or just use the same text with session_id
            // Usually, the session_id on the server tracks the state.
            vm.getAIResponse(msg.originalText, msg.sessionId, additionalData).then(function (response) {
                // ... same success handling logic as in sendMessage ...
                // To avoid code duplication, we could extract the response handler, 
                // but for now, I'll repeat the basic parsing logic since we need the same JSON cleanup.

                vm.isLoading = false;
                processingMsg.isLoading = false;
                var rawData = response.data.replace(/^'''|'''$/g, '');
                var apiData = JSON.parse(rawData);

                var assistantText = (apiData.result && apiData.result.desc) || apiData.desc || 'Request processed.';
                vm.messages.push({ type: 'assistant', text: assistantText, isTable: false });

                if (apiData.status === "success" && apiData.result && apiData.result.data) {
                    var tableData = vm.formatTableData(apiData.result.data);
                    vm.messages.push({
                        type: 'assistant',
                        text: '',
                        isTable: true,
                        table: tableData,
                        isCollapsed: false,
                        vizType: 'table',
                        showMenu: false,
                        rawData: apiData.result.data,
                        graphKeys: apiData.result.graph_keys,
                        selectedKeyIndex: 0,
                        showKeySelector: false
                    });
                } else if (apiData.status === "multiple_matches") {
                    // Handle recursive matches if any
                    vm.messages.push({
                        type: 'assistant', text: 'More clarification needed:', isOptions: true,
                        options: apiData.options, sessionId: apiData.session_id,
                        originalText: apiData.text, selections: {}
                    });
                }
                vm.scrollToBottom();
            }, function (error) {









                vm.isLoading = false;
                processingMsg.isLoading = false;
                vm.messages.push({ type: 'assistant', text: 'Error executing request.', isTable: false });
                vm.scrollToBottom();
            });
        };

        // Change visualization type
        vm.changeViz = function (msg, type) {
            msg.vizType = type;
            msg.showMenu = false;
            if (type !== 'table' && msg.graphKeys && msg.graphKeys.length > 1) {
                msg.showKeySelector = true;
            } else {
                msg.showKeySelector = false;
            }
            console.log("Changed visualization to: " + type);
        };

        // Change the selected graph key pair
        vm.changeKey = function (msg, index) {
            msg.selectedKeyIndex = index;
            msg.showKeySelector = false;
        };

        // Global click listener to close visualization menus
        document.addEventListener('click', function () {
            vm.messages.forEach(function (m) {
                if (m.isTable) {
                    m.showMenu = false;
                    m.showKeySelector = false;
                }
            });
            $scope.$apply();
        });

        // Handle Enter key for sending message
        vm.handleKeyPress = function (event) {
            if (event.keyCode === 13) {
                vm.sendMessage();
            }
        };

        vm.RefreshData = function () {
            vm.messages = [
                {
                    type: 'assistant',
                    text: 'Hello! neo-assistant here, your digital sidekick, at your service!',
                    isTable: false
                }
            ];
            vm.userPrompt = "";
            vm.isLoading = false;
            console.log("Chat balanced and refreshed.");
        };
    }

    // Make toggleSubmenu available globally for onclick handlers
    window.toggleSubmenu = function (event, submenuId) {
        var scope = angular.element(document.querySelector('[ng-controller="aiAssistantCtrl"]')).scope();
        if (scope && scope.vm) {
            scope.vm.toggleSubmenu(event, submenuId);
            scope.$apply();
        }
    };

    // AI Chart Directive for Chart.js 1.0.2
    angular.module('aiAssistantApp')
        .directive('aiChart', ['$timeout', function ($timeout) {
            return {
                restrict: 'A',
                scope: {
                    vizType: '=',
                    rawData: '=',
                    graphKeys: '=',
                    legendData: '=',
                    selectedIndex: '='
                },
                link: function (scope, element, attrs) {
                    var chartInstance = null;
                    var colors = ['#7ab55c', '#4a90e2', '#f39c12', '#e74c3c', '#9b59b6', '#1abc9c', '#34495e', '#16a085'];
                    var highlights = ['#95d674', '#64a9f5', '#ffb74d', '#ff7061', '#b388ff', '#4db6ac', '#5d6d7e', '#1abc9c'];

                    function renderChart() {
                        if (chartInstance) {
                            chartInstance.destroy();
                        }

                        if (!scope.rawData || !scope.graphKeys || scope.graphKeys.length === 0) return;
                        if (scope.vizType === 'table') return;

                        var ctx = element[0].getContext('2d');
                        var idx = scope.selectedIndex || 0;
                        var keys = scope.graphKeys[idx]; // [labelKey, valueKey]
                        var labels = scope.rawData[keys[0]];
                        var values = scope.rawData[keys[1]];

                        // Reset legend data
                        scope.legendData = [];

                        if (scope.vizType === 'pie') {
                            var pieData = [];
                            for (var i = 0; i < labels.length; i++) {
                                var val = parseFloat(values[i]);
                                pieData.push({
                                    value: val,
                                    color: colors[i % colors.length],
                                    highlight: highlights[i % highlights.length],
                                    label: labels[i]
                                });
                                // Add to legend
                                scope.legendData.push({
                                    label: labels[i],
                                    value: val,
                                    color: colors[i % colors.length]
                                });
                            }
                            chartInstance = new Chart(ctx).Pie(pieData, { responsive: true, maintainAspectRatio: false });
                        } else if (scope.vizType === 'column' || scope.vizType === 'line') {
                            var chartData = {
                                labels: labels,
                                datasets: [{
                                    label: keys[1],
                                    fillColor: "rgba(122, 181, 92, 0.5)",
                                    strokeColor: "rgba(122, 181, 92, 0.8)",
                                    highlightFill: "rgba(122, 181, 92, 0.75)",
                                    highlightStroke: "rgba(122, 181, 92, 1)",
                                    pointColor: "rgba(122, 181, 92, 1)",
                                    pointStrokeColor: "#fff",
                                    pointHighlightFill: "#fff",
                                    pointHighlightStroke: "rgba(122, 181, 92, 1)",
                                    data: values.map(function (v) { return parseFloat(v); })
                                }]
                            };

                            if (scope.vizType === 'column') {
                                for (var j = 0; j < labels.length; j++) {
                                    scope.legendData.push({
                                        label: labels[j],
                                        value: parseFloat(values[j]),
                                        color: "rgba(122, 181, 92, 0.8)"
                                    });
                                }
                                chartInstance = new Chart(ctx).Bar(chartData, { responsive: true, maintainAspectRatio: false, barShowStroke: true });
                            } else {
                                chartInstance = new Chart(ctx).Line(chartData, { responsive: true, maintainAspectRatio: false });
                            }
                        }
                    }

                    scope.$watchGroup(['vizType', 'selectedIndex'], function (newVals) {
                        if (newVals[0] && newVals[0] !== 'table') {
                            $timeout(renderChart, 100);
                        }
                    });
                }
            };
        }]);

})();
