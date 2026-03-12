




//@* api / Indicatorpanel / Overview *@
//    @* fromDate: @Html.Raw(Json.Encode(ViewBag.StartFromDate)),
//toDate: @Html.Raw(Json.Encode(ViewBag.EndToDate)),*@

/*@*const BASE_URL = @Html.Raw(Json.Encode(ViewBag.BaseUrlForDashboard));*/
//function getSalesData(selectedMonth) {
//    const url = `${BASE_URL}/api/Indicatorpanel/Overview`;
//    const requestData = {
//        repType: "Overview",
//        branchCode: @Html.Raw(Json.Encode(ViewBag.BranchCode)),
//    companyCode: @Html.Raw(Json.Encode(ViewBag.CompanyCode)),
//    UserId: @Html.Raw(Json.Encode(ViewBag.UserId)), -->
//        fromDate: @Html.Raw(Json.Encode(ViewBag.StartFromDate)),
//    toDate: @Html.Raw(Json.Encode(ViewBag.EndToDate)),
//    < !--fromDate: "17-Jul-2024",
//        toDate: "16-Jul-2025",
//            itemStringFilter: [],
//                customerCode: "",
//                    dealerCode: "",
//                        months: [selectedMonth],
//                            cat: "",
//                                brand: "",
//                                    grp: "",
//                                        grossNet: "",
//                                            grossNetStr: "",
//                                                brand: "",
//                                                    cusGrp: "",
//                                                        cusGrpStr: "",
//                                                            unitType: "",
//                                                                with_eng_date: ""
//};

//fetch(url, {
//    method: "POST",
//    headers: {
//        "Content-Type": "application/json"
//    },
//    body: JSON.stringify(requestData)
//})
//    .then(response => response.json())
//    .then(data => {
//        console.log("API Response:", data);
//        data.forEach(item => {
//            switch (item.topic) {
//                case "Sales":
//                    document.getElementById("salesAmount").textContent = parseInt(item.amt);
//                    break;

//                case "Collection":
//                    document.getElementById("collectionAmount").textContent = parseInt(item.amt);
//                    break;

//                case "Purchase":
//                    document.getElementById("purchaseAmount").textContent = parseInt(item.amt);
//                    break;

//                case "Production":
//                    document.getElementById("productionAmount").textContent = parseInt(item.amt);
//                    break;

//                case "Cash Balance":
//                    document.getElementById("cashBalanceAmount").textContent = parseInt(item.amt);
//                    break;

//                case "Bank Balance":
//                    document.getElementById("bankBalanceAmount").textContent = parseInt(item.amt);
//                    break;

//                case "Receivable":
//                    document.getElementById("receivableAmount").textContent = parseInt(item.amt);
//                    break;

//                case "Payable":
//                    document.getElementById("payableAmount").textContent = parseInt(item.amt);
//                    break;

//                case "Loan":
//                    document.getElementById("loanAmount").textContent = parseInt(item.amt);
//                    break;

//                case "VAT":
//                    document.getElementById("vatAmount").textContent = parseInt(item.amt);
//                    break;

//                case "Expenses":
//                    document.getElementById("expensesAmount").textContent = parseInt(item.amt);
//                    break;

//                case "ClosingStock":
//                    document.getElementById("closingStockAmount").textContent = parseInt(item.amt);
//                    break;
//            }
//        });
//    })
//    .catch(error => {
//        console.error("Error fetching sales data:", error);
//    });
//        }

//const BASE_URL = @Html.Raw(Json.Encode(ViewBag.BaseUrlForDashboard));
//function getSalesData(selectedMonth) {
//    const url = `${BASE_URL}/api/Indicatorpanel/Overview`;
//    const requestData = {
//        repType: "Overview",
//        branchCode: @Html.Raw(Json.Encode(ViewBag.BranchCode)),
//    companyCode: @Html.Raw(Json.Encode(ViewBag.CompanyCode)),
//    UserId: @Html.Raw(Json.Encode(ViewBag.UserId)),
//    //fromDate: "17-Jul-2024",
//    //toDate: "16-Jul-2025",
//    fromDate: @Html.Raw(Json.Encode(ViewBag.StartFromDate)),
//    toDate: @Html.Raw(Json.Encode(ViewBag.EndToDate)),
//    itemStringFilter: [],
//        customerCode: "",
//            dealerCode: "",
//                months: [selectedMonth],
//                    cat: "",
//                        brand: "",
//                            grp: "",
//                                grossNet: "",
//                                    grossNetStr: "",
//                                        cusGrp: "",
//                                            cusGrpStr: "",
//                                                unitType: "",
//                                                    with_eng_date: ""
//};

console.log("Sending request:", requestData);

fetch(url, {
    method: "POST",
    headers: {
        "Content-Type": "application/json"
    },
    body: JSON.stringify(requestData)
})
    .then(response => response.json())
    .then(data => {
        /* console.log("API Response:", data);*/
        // Create an object to sum up duplicate topics
        const topicTotals = {};

        data.forEach(item => {
            if (!topicTotals[item.topic]) {
                topicTotals[item.topic] = { amt: 0, qty: 0 };
            }
            topicTotals[item.topic].amt += item.amt || 0;
            topicTotals[item.topic].qty += item.qty || 0;
        });

        // Now update UI
        Object.keys(topicTotals).forEach(topic => {
            let elementId = "";
            switch (topic) {
                case "Sales": elementId = "salesAmount"; break;
                case "Collection": elementId = "collectionAmount"; break;
                case "Purchase": elementId = "purchaseAmount"; break;
                case "Production": elementId = "productionAmount"; break;
                case "Cash Balance": elementId = "cashBalanceAmount"; break;
                case "Bank Balance": elementId = "bankBalanceAmount"; break;
                case "Receivable": elementId = "receivableAmount"; break;
                case "Payable": elementId = "payableAmount"; break;
                case "Loan": elementId = "loanAmount"; break;
                case "VAT": elementId = "vatAmount"; break;
                case "Expenses": elementId = "expensesAmount"; break;
                case "ClosingStock": elementId = "closingStockAmount"; break;
            }

            if (elementId != null) {
                const element = document.getElementById(elementId);
                if (element != null) {
                    //console.log(element);
                    element.textContent = topicTotals[topic].amt ? parseInt(topicTotals[topic].amt) : 'N/A';
                }
                if (element == null) {
                    if (elementId == 'productionAmount') {
                        const productionAmountValue = document.getElementById("productionAmount");
                        productionAmountValue.values = 'N/A';
                        //document.getElementById("productionAmount").value= 'N/A';
                    }
                    if (elementId == 'closingStockAmount') {
                        const closingStockAmountValue = document.getElementById("closingStockAmount");
                        closingStockAmountValue.values = 'N/A';
                        //document.getElementById("closingStockAmount").value= 'N/A';
                    }
                }
            }
        });

    })
    .catch(error => {
        console.error("Error fetching sales data:", error);
    });
    }

// Event listener for dropdown item selection
//document.querySelectorAll(".dropdown-item").forEach(item => {
//    item.addEventListener("click", function (event) {
//        event.preventDefault();
//          selectedMonth = this.textContent;
//        getSalesData(selectedMonth);
//    });
//});

/* window.onload = getSalesData;*/
window.addEventListener('load', () => {
    var selectedMonth = 'This Month';
    getSalesData(selectedMonth);
    loadDropdownData();
    fetchSalesTodayYTDMTD();
    fetchProductWiseSales();
    //fetchDealerCustomerWiseSales();
    fetchGraphWiseSalesReport();
    //fetchGraphWiseSalesReportForKendoUI();
    fetchSalesData();
});


/*@* /api/Indicatorpanel / GetSalesTodayYTDMTD *@*/

const BASE_URL_Sales = @Html.Raw(Json.Encode(ViewBag.BaseUrlForDashboard));
async function fetchSalesTodayYTDMTD() {
    const apiUrl = `${BASE_URL_Sales}/api/Indicatorpanel/GetSalesTodayYTDMTD`;
    const params = {
        repType: "GetSalesTodayYTDMTD",
        branchCode: @Html.Raw(Json.Encode(ViewBag.BranchCode)),
    companyCode: @Html.Raw(Json.Encode(ViewBag.CompanyCode)),
    UserId: @Html.Raw(Json.Encode(ViewBag.UserId)),
    //fromDate: "17-Jul-2024",
    //toDate: "16-Jul-2025",
    fromDate: @Html.Raw(Json.Encode(ViewBag.StartFromDate)),
    toDate: @Html.Raw(Json.Encode(ViewBag.EndToDate)),
    itemStringFilter: [],
        customerCode: "",
            dealerCode: "",
                months: ["This Month", "This Year", "Last Month"],
                    cat: "",
                        brand: "",
                            grp: "",
                                grossNet: "",
                                    grossNetStr: "",
                                        cusGrp: "",
                                            cusGrpStr: "",
                                                unitType: "",
                                                    with_eng_date: ""
};
try {
    const response = await fetch(apiUrl, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(params)
    });

    if (!response.ok) {
        throw new Error('Network response was not ok');
    }
    const data = await response.json();
    //console.log(data);
    updateSalesDataSalesTodayYTDMTD(data);
} catch (error) {
    console.error('Error fetching sales data:', error);
}
}
function updateSalesDataSalesTodayYTDMTD(data) {

    // Convert object to array
    const dataArray = Object.entries(data).map(([key, value]) => ({ key, value }));
    const weekDaysSales = dataArray
        .filter(item => item.key.match(/^sales_(MON|TUE|WED|THU|FRI|SAT|SUN)-\d{2}\/\d{2}$/))
        .map(item => {
            const [day, date] = item.key.replace("sales_", "").split("-");
            return {
                day,
                date,
                ...item.value
            };
        });

    const withOutWeekDaysSales = dataArray
        .filter(item => !item.key.match(/^sales_(MON|TUE|WED|THU|FRI|SAT|SUN)-\d{2}\/\d{2}$/))
        .map(item => ({
            key: item.key,
            ...item.value
        }));


    console.log(weekDaysSales);
    console.log(withOutWeekDaysSales);


    if (weekDaysSales.length > 0) {
        if (weekDaysSales.length > 0) {
            if (weekDaysSales[0].date != '' && parseInt(weekDaysSales[0].totalAmt) > 0) {
                document.getElementById('Weekh2dYeserday').innerText = weekDaysSales[0].date;
                document.getElementById('Weekp2dYeserday').innerText = parseInt(weekDaysSales[0].TotalAmt);
            }
            else {
                document.getElementById('Weekh2dYeserday').innerText = weekDaysSales[0].date;
                document.getElementById('Weekp2dYeserday').innerText = parseInt(weekDaysSales[0].TotalAmt);
            }
        }
        if (weekDaysSales.length > 1) {
            if (weekDaysSales[1].date != '' && parseInt(weekDaysSales[1].totalAmt) > 0) {
                document.getElementById('Weekh2dToday').innerText = weekDaysSales[1].date;
                document.getElementById('Weekp2dToday').innerText = parseInt(weekDaysSales[1].TotalAmt);
            }
            else {
                document.getElementById('Weekh2dToday').innerText = weekDaysSales[1].date;
                document.getElementById('Weekp2dToday').innerText = parseInt(weekDaysSales[1].TotalAmt);
            }
        }
        if (weekDaysSales[2].date != '' && parseInt(weekDaysSales[2].totalAmt) > 0) {
            document.getElementById('Weekh2MTDToday').innerText = weekDaysSales[2].date;
            document.getElementById('Weekp2MTDToday').innerText = parseInt(weekDaysSales[2].TotalAmt);
        }
        else {
            document.getElementById('Weekh2MTDToday').innerText = weekDaysSales[2].date;
            document.getElementById('Weekp2MTDToday').innerText = parseInt(weekDaysSales[2].TotalAmt);
        }
        if (weekDaysSales[3].date != '' && parseInt(weekDaysSales[3].totalAmt) > 0) {
            document.getElementById('Weekh2YTD').innerText = weekDaysSales[3].date;
            document.getElementById('WeekP2YTD').innerText = parseInt(weekDaysSales[3].TotalAmt);
        }
        else {
            document.getElementById('Weekh2YTD').innerText = weekDaysSales[3].date;
            document.getElementById('WeekP2YTD').innerText = parseInt(weekDaysSales[3].TotalAmt);
        }
        if (weekDaysSales.length > 4) {
            if (weekDaysSales[4].date != '' && weekDaysSales[4].totalAmt > 0) {
                document.getElementById('Weekh3dToday').innerText = weekDaysSales[4].date;
                document.getElementById('Weekp3dToday').innerText = parseInt(weekDaysSales[4].TotalAmt);
            }
            else {
                document.getElementById('Weekh3dToday').innerText = weekDaysSales[4].date;
                document.getElementById('Weekp3dToday').innerText = parseInt(weekDaysSales[4].TotalAmt);
            }
        }
        if (weekDaysSales.length > 5) {
            if (weekDaysSales[5].date != '' && weekDaysSales[5].totalAmt > 0) {
                document.getElementById('Weekh3MTDToday').innerText = weekDaysSales[5].date;
                document.getElementById('Weekp3MTDToday').innerText = parseInt(weekDaysSales[5].TotalAmt);
            }
            else {
                document.getElementById('Weekh3MTDToday').innerText = weekDaysSales[5].date;
                document.getElementById('Weekp3MTDToday').innerText = parseInt(weekDaysSales[5].TotalAmt);
            }
        }
        if (weekDaysSales.length > 6) {
            if (weekDaysSales[6].date != '' && weekDaysSales[6].totalAmt > 0) {
                document.getElementById('Weekh3YTD').innerText = weekDaysSales[6].date;
                document.getElementById('WeekP3YTD').innerText = parseInt(weekDaysSales[6].TotalAmt);
            }
            else {
                document.getElementById('Weekh3YTD').innerText = weekDaysSales[6].date;
                document.getElementById('WeekP3YTD').innerText = parseInt(weekDaysSales[6].TotalAmt);
            }
        }
    }

    if (withOutWeekDaysSales.length > 0) {
        if (withOutWeekDaysSales.length > 0) {
            if (withOutWeekDaysSales[0].key == 'sales_This Year') {
                document.getElementById('h3YTD').innerText = 'This Year';
                document.getElementById('P3YTD').innerText = parseInt(withOutWeekDaysSales[0].TotalAmt);
            }
            else {
                document.getElementById('h3YTD').innerText = 'This Year';
                document.getElementById('P3YTD').innerText = 'N/A';
            }
        }
        else {
            document.getElementById('h3YTD').innerText = 'This Year';
            document.getElementById('P3YTD').innerText = 'N/A';
        }
        if (withOutWeekDaysSales.length > 1) {
            if (withOutWeekDaysSales[1].key == 'sales_This_Month') {
                document.getElementById('h1MTDToday').innerText = 'This Month';
                document.getElementById('p1MTDToday').innerText = parseInt(withOutWeekDaysSales[1].TotalAmt);
            }
            else {
                document.getElementById('h1MTDToday').innerText = 'This Month';
                document.getElementById('p1MTDToday').innerText = 'N/A';
            }
        }
        else {
            document.getElementById('h1MTDToday').innerText = 'This Month';
            document.getElementById('p1MTDToday').innerText = 'N/A';
        }
        if (withOutWeekDaysSales.length > 2) {
            if (withOutWeekDaysSales[2].key == 'sales_Yesterday') {
                document.getElementById('h1dYeserday').innerText = 'Yesterday';
                document.getElementById('p1dYeserday').innerText = parseInt(withOutWeekDaysSales[2].TotalAmt);
            }
            else {
                document.getElementById('h1dYeserday').innerText = 'Yesterday';
                document.getElementById('p1dYeserday').innerText = 'N/A';
            }
        }
        else {
            document.getElementById('h1dYeserday').innerText = 'Yesterday';
            document.getElementById('p1dYeserday').innerText = 'N/A';
        }
        if (withOutWeekDaysSales.length > 3) {
            if (withOutWeekDaysSales[3].key == 'sales_Today') {
                document.getElementById('h1dToday').innerText = 'Today';
                document.getElementById('p1dToday').innerText = parseInt(withOutWeekDaysSales[3].TotalAmt);
            }
            else {
                document.getElementById('h1dToday').innerText = 'Today';
                document.getElementById('p1dToday').innerText = 'N/A';
            }
        }
        else {
            document.getElementById('h1dToday').innerText = 'Today';
            document.getElementById('p1dToday').innerText = 'N/A';
        }
    }
}


//@* /api/Indicatorpanel / GetProductWiseSalesReports *@
//    @* product_wise_sales
//monthly_sales_vs_collection *@


async function fetchProductWiseSales() {
    const BASE_URL_ProductWiseSales = @Html.Raw(Json.Encode(ViewBag.BaseUrlForDashboard));
    const params = {
        repType: "product_wise_sales",
        branchCode: @Html.Raw(Json.Encode(ViewBag.BranchCode)),
    companyCode: @Html.Raw(Json.Encode(ViewBag.CompanyCode)),
    UserId: @Html.Raw(Json.Encode(ViewBag.UserId)),
    @* fromDate: @Html.Raw(Json.Encode(ViewBag.StartFromDate)),
    toDate: @Html.Raw(Json.Encode(ViewBag.EndToDate)),*@
    //fromDate: "17-Jul-2024",
    //toDate: "16-Jul-2025",
    fromDate: @Html.Raw(Json.Encode(ViewBag.StartFromDate)),
    toDate: @Html.Raw(Json.Encode(ViewBag.EndToDate)),
    itemStringFilter: [],
        customerCode: "",
            dealerCode: "",
                months: ["This Month", "This Year", "Last Month"],
                    cat: "",
                        brand: "",
                            grp: "",
                                grossNet: "",
                                    grossNetStr: "",
                                        brand: "",
                                            cusGrp: "",
                                                cusGrpStr: "",
                                                    unitType: "",
                                                        with_eng_date: ""
};
const BASE_URLApiUrlProductWiseSales = `${BASE_URL_ProductWiseSales}/api/Indicatorpanel/GetProductWiseSalesReports`;
try {
    const response = await fetch(BASE_URLApiUrlProductWiseSales, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(params)
    });

    if (!response.ok) {
        throw new Error('Network response was not ok');
    }

    const data = await response.json();
    //bindProductWiseSales(data.product_wise_report);
} catch (error) {
    console.error('There was a problem with the fetch operation:', error);
}
        }

async function fetchMonthlySalesVsCollectionReport() {
    const BASE_URL_ProductWiseSales = @Html.Raw(Json.Encode(ViewBag.BaseUrlForDashboard));
    const params = {
        repType: "monthly_sales_vs_collection",
        branchCode: @Html.Raw(Json.Encode(ViewBag.BranchCode)),
    companyCode: @Html.Raw(Json.Encode(ViewBag.CompanyCode)),
    UserId: @Html.Raw(Json.Encode(ViewBag.UserId)),
    @* fromDate: @Html.Raw(Json.Encode(ViewBag.StartFromDate)),
    toDate: @Html.Raw(Json.Encode(ViewBag.EndToDate)),*@
    //fromDate: "17-Jul-2023",
    //toDate: "16-Jul-2024",
    fromDate: @Html.Raw(Json.Encode(ViewBag.StartFromDate)),
    toDate: @Html.Raw(Json.Encode(ViewBag.EndToDate)),
    itemStringFilter: [],
        customerCode: "",
            dealerCode: "",
                months: ["This Month", "This Year", "Last Month"],
                    cat: "",
                        brand: "",
                            grp: "",
                                grossNet: "",
                                    grossNetStr: "",
                                        brand: "",
                                            cusGrp: "",
                                                cusGrpStr: "",
                                                    unitType: "",
                                                        with_eng_date: ""
};
const BASE_URLApiUrlProductWiseSales = `${BASE_URL_ProductWiseSales}/api/Indicatorpanel/GetMonthlySalesVsCollectionReport`;
try {
    const response = await fetch(BASE_URLApiUrlProductWiseSales, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(params)
    });

    if (!response.ok) {
        throw new Error('Network response was not ok');
    }

    const data = await response.json();
    // Prepare Data for Charts
    const months = data.detail.map((item) => item.title);
    const sales = data.detail.map((item) => item.sales);
    const collections = data.detail.map((item) => item.collection);

    // Line Chart
    new Chart(document.getElementById("lineChart"), {
        type: "line",
        data: {
            labels: months,
            datasets: [
                { label: "Sales", data: sales, borderColor: "blue", fill: false },
                { label: "Collections", data: collections, borderColor: "green", fill: false }
            ]
        }
    });

    //bindProductWiseSales(data.product_wise_report);
} catch (error) {
    console.error('There was a problem with the fetch operation:', error);
}
            }
function bindProductWiseSales(salesData) {
    const labels = salesData.map(item => item.item);
    const salesQty = salesData.map(item => item.net_sales_qty);
    const ctx = document.getElementById('areaChart').getContext('2d');
    const areaChart = new Chart(ctx, {
        type: 'line',
        data: {
            labels: labels,
            datasets: [{
                label: 'Net Sales Quantity',
                data: salesQty,
                backgroundColor: 'rgba(75, 192, 192, 0.4)',
                borderColor: 'rgba(75, 192, 192, 1)',
                borderWidth: 1,
                fill: true
            }]
        },
        options: {
            responsive: true,
            scales: {
                y: {
                    beginAtZero: true,
                    title: {
                        display: true,
                        text: 'Quantity'
                    }
                },
                x: {
                    title: {
                        display: true,
                        text: 'Products'
                    }
                }
            }
        }
    });
}

//function bindProductWiseSales(salesData) {
//    const labels = salesData.map(item => item.item);
//    const salesQty = salesData.map(item => item.net_sales_qty);

//    // 1. Line Chart
//    const lineChartCanvas = document.getElementById('lineChartCanvas').getContext('2d');
//    new Chart(lineChartCanvas, {
//        type: 'line',
//        data: {
//            labels: labels,
//            datasets: [{
//                label: 'Net Sales Quantity',
//                data: salesQty,
//                backgroundColor: 'rgba(75, 192, 192, 0.2)',
//                borderColor: 'rgba(75, 192, 192, 1)',
//                fill: true
//            }]
//        },
//        options: {
//            responsive: true,
//            scales: {
//                y: {
//                    beginAtZero: true,
//                    title: {
//                        display: true,
//                        text: 'Quantity'
//                    }
//                },
//                x: {
//                    title: {
//                        display: true,
//                        text: 'Products'
//                    }
//                }
//            }
//        }
//    });

//    // 2. Pie Chart
//    const pieChartCanvas = document.getElementById('pieChartCanvas').getContext('2d');
//    new Chart(pieChartCanvas, {
//        type: 'pie',
//        data: {
//            labels: labels,
//            datasets: [{
//                data: salesQty,
//                backgroundColor: ['#f56954', '#00a65a', '#f39c12', '#00c0ef', '#3c8dbc']
//            }]
//        }
//    });

//    // 3. Radar Chart
//    const radarChartCanvas = document.getElementById('radarChartCanvas').getContext('2d');
//    new Chart(radarChartCanvas, {
//        type: 'radar',
//        data: {
//            labels: labels,
//            datasets: [{
//                label: 'Net Sales Quantity',
//                data: salesQty,
//                backgroundColor: 'rgba(60,141,188,0.2)',
//                borderColor: 'rgba(60,141,188,1)'
//            }]
//        }
//    });

//    // 4. Polar Area Chart
//    const polarAreaChartCanvas = document.getElementById('polarAreaChartCanvas').getContext('2d');
//    new Chart(polarAreaChartCanvas, {
//        type: 'polarArea',
//        data: {
//            labels: labels,
//            datasets: [{
//                data: salesQty,
//                backgroundColor: ['#f56954', '#00a65a', '#f39c12', '#00c0ef', '#3c8dbc']
//            }]
//        }
//    });

//    // 5. Scatter Chart
//    const scatterChartCanvas = document.getElementById('scatterChartCanvas').getContext('2d');
//    new Chart(scatterChartCanvas, {
//        type: 'scatter',
//        data: {
//            datasets: [{
//                label: 'Sales Scatter Plot',
//                data: salesData.map(item => ({x: item.item, y: item.net_sales_qty })),
//                backgroundColor: '#00c0ef'
//            }]
//        },
//        options: {
//            scales: {
//                x: {
//                    type: 'category',
//                },
//                y: {
//                    beginAtZero: true
//                }
//            }
//        }
//    });

//    // 6. Bubble Chart
//    const bubbleChartCanvas = document.getElementById('bubbleChartCanvas').getContext('2d');
//    new Chart(bubbleChartCanvas, {
//        type: 'bubble',
//        data: {
//            datasets: [{
//                label: 'Product Sales',
//                data: salesData.map(item => ({x: item.item, y: item.net_sales_qty, r: item.net_sales_qty / 10 })),
//                backgroundColor: '#00a65a'
//            }]
//        }
//    });

//    // 7. Bar Chart
//    const barChartCanvas = document.getElementById('barChartCanvas').getContext('2d');
//    new Chart(barChartCanvas, {
//        type: 'bar',
//        data: {
//            labels: labels,
//            datasets: [{
//                label: 'Net Sales Quantity',
//                data: salesQty,
//                backgroundColor: 'rgba(0,123,255,0.7)'
//            }]
//        }
//    });

//    // 8. Stacked Bar Chart
//    const stackedBarChartCanvas = document.getElementById('stackedBarChartCanvas').getContext('2d');
//    new Chart(stackedBarChartCanvas, {
//        type: 'bar',
//        data: {
//            labels: labels,
//            datasets: [{
//                label: 'Category 1',
//                data: salesQty,
//                backgroundColor: '#3c8dbc'
//            }, {
//                label: 'Category 2',
//                data: salesQty.map(qty => qty / 2),
//                backgroundColor: '#00c0ef'
//            }]
//        },
//        options: {
//            scales: {
//                x: {stacked: true },
//                y: {stacked: true }
//            }
//        }
//    });

//    // 9. Horizontal Bar Chart
//    const horizontalBarChartCanvas = document.getElementById('horizontalBarChartCanvas').getContext('2d');
//    new Chart(horizontalBarChartCanvas, {
//        type: 'bar',
//        data: {
//            labels: labels,
//            datasets: [{
//                label: 'Net Sales Quantity',
//                data: salesQty,
//                backgroundColor: 'rgba(0,123,255,0.7)'
//            }]
//        },
//        options: {
//            indexAxis: 'y',
//            scales: {
//                x: {
//                    beginAtZero: true
//                }
//            }
//        }
//    });

//    // 10. Mixed Chart (Bar + Line)
//    const mixedChartCanvas = document.getElementById('mixedChartCanvas').getContext('2d');
//    new Chart(mixedChartCanvas, {
//        type: 'bar',
//        data: {
//            labels: labels,
//            datasets: [{
//                label: 'Bar Dataset',
//                data: salesQty,
//                backgroundColor: '#f39c12',
//                type: 'bar'
//            }, {
//                label: 'Line Dataset',
//                data: salesQty.map(qty => qty * 0.8),
//                type: 'line',
//                borderColor: '#00a65a',
//                backgroundColor: 'rgba(0, 166, 90, 0.1)'
//            }]
//        }
//    });
//}




@* async function fetchDealerCustomerWiseSales() {
    const BASE_URL_DealerCustomerWiseSales = @Html.Raw(Json.Encode(ViewBag.BaseUrlForDashboard));
    const params = {
        branchCode: @Html.Raw(Json.Encode(ViewBag.BranchCode)),
    companyCode: @Html.Raw(Json.Encode(ViewBag.CompanyCode)),
    userId: @Html.Raw(Json.Encode(ViewBag.UserId)),
    fromDate: @Html.Raw(Json.Encode(ViewBag.StartFromDate)),
    toDate: @Html.Raw(Json.Encode(ViewBag.EndToDate)),
    itemCode: [],
        customerCode: "",
            months: ["This Month", "This Year", "Last Month"],
                repType: "dealer_customer_wise_sales",
                    cat: "",
                        brand: "",
                            grp: ""
};

const BASE_URLApiUrlDealerCustomerWise = `${BASE_URL_DealerCustomerWiseSales}/api/Indicatorpanel/GetDealerCustomerWiseSalesReport`;

try {
    const response = await fetch(BASE_URLApiUrlDealerCustomerWise, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(params)
    });

    if (!response.ok) {
        throw new Error('Network response was not ok');
    }

    const data = await response.json();
    bindDealerCustomerWiseSales(data.detail);

} catch (error) {
    console.error('There was a problem with the fetch operation:', error);
}
        }

function bindDealerCustomerWiseSales(firstQueryResults) {
    const labels = [];
    const salesData = [];

    firstQueryResults.forEach(result => {
        labels.push(result.name);
        salesData.push(result.sales_qty);
    });

    var lineChartCanvas = $('#lineChart').get(0).getContext('2d');
    new Chart(lineChartCanvas, {
        type: 'line',
        data: {
            labels: labels,
            datasets: [{
                label: 'Dealer Customer Wise Sales Quantity',
                data: salesData,
                borderColor: 'rgba(54, 162, 235, 1)',
                backgroundColor: 'rgba(54, 162, 235, 0.2)',
                fill: true
            }]
        },
        options: {
            responsive: true,
            scales: {
                x: {
                    title: {
                        display: true,
                        text: 'Companies'
                    }
                },
                y: {
                    title: {
                        display: true,
                        text: 'Sales Quantity'
                    },
                    beginAtZero: true
                }
            }
        }
    });
}
document.addEventListener('DOMContentLoaded', fetchDealerCustomerWiseSales);*@
    </script >

    /*@* /api/Indicatorpanel / GetGraphWiseSalesRepor *@*/

    async function fetchGraphWiseSalesReport() {
        const BASE_URL = @Html.Raw(Json.Encode(ViewBag.BaseUrlForDashboard));
        try {
            const requestData = {
                repType: "graph_wise_sales",
                branchCode: @Html.Raw(Json.Encode(ViewBag.BranchCode)),
            companyCode: @Html.Raw(Json.Encode(ViewBag.CompanyCode)),
            UserId: @Html.Raw(Json.Encode(ViewBag.UserId)),
            //fromDate: "17-Jul-2024",
            //toDate: "16-Jul-2025",
            fromDate: @Html.Raw(Json.Encode(ViewBag.StartFromDate)),
            toDate: @Html.Raw(Json.Encode(ViewBag.EndToDate)),
            itemStringFilter: [],
                customerCode: "",
                    dealerCode: "",
                        months: ["This Month", "This Year", "Last Month"],
                            cat: "",
                                brand: "",
                                    grp: "",
                                        grossNet: "",
                                            grossNetStr: "",
                                                brand: "",
                                                    cusGrp: "",
                                                        cusGrpStr: "",
                                                            unitType: "",
                                                                with_eng_date: ""
        };

        const response = await fetch(`${BASE_URL}/api/Indicatorpanel/GetGraphWiseSalesReport`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(requestData)
        });


        const data = await response.json();
        const labelSalesChart = data.branch_wise_sales.details.map(branch => branch.name);
        const amountSalesChart = data.branch_wise_sales.details.map(branch => branch.amt);
        const quantitieSalesChart = data.branch_wise_sales.details.map(branch => branch.qty);
        const ctxSalesChart = document.getElementById('salesChart').getContext('2d');
        const salesChart = new Chart(ctxSalesChart, {
            type: 'bar',
            data: {
                labels: labelSalesChart,
                datasets: [
                    {
                        label: 'Sales Amount',
                        data: amountSalesChart,
                        backgroundColor: {
                            type: 'linear',
                            colors: [
                                { offset: 0, color: 'rgba(58, 123, 213, 0.8)' },
                                { offset: 1, color: 'rgba(58, 123, 213, 0.4)' },
                            ],
                        },
                        borderColor: 'rgba(58, 123, 213, 1)',
                        borderWidth: 2,
                        borderRadius: 4,
                        hoverBackgroundColor: 'rgba(58, 123, 213, 1)',
                    },
                    {
                        label: 'Quantity Sold',
                        data: quantitieSalesChart,
                        backgroundColor: {
                            type: 'linear',
                            colors: [
                                { offset: 0, color: 'rgba(255, 159, 64, 0.8)' },
                                { offset: 1, color: 'rgba(255, 159, 64, 0.4)' },
                            ],
                        },
                        borderColor: 'rgba(255, 159, 64, 1)',
                        borderWidth: 2,
                        borderRadius: 4,
                        hoverBackgroundColor: 'rgba(255, 159, 64, 1)',
                    }
                ],
                options: {
                    scales: {
                        x: {
                            grid: {
                                color: 'rgba(255, 255, 255, 0.1)',
                            },
                            ticks: {
                                color: 'white',
                            }
                        },
                        y: {
                            grid: {
                                color: 'rgba(255, 255, 255, 0.1)',
                            },
                            ticks: {
                                color: 'white',
                            }
                        }
                    },
                    plugins: {
                        legend: {
                            labels: {
                                color: 'white'
                            }
                        },
                        tooltip: {
                            backgroundColor: 'rgba(0, 0, 0, 0.7)',
                            titleColor: 'white',
                            bodyColor: 'white',
                            borderColor: 'rgba(75, 192, 192, 1)',
                            borderWidth: 1,
                        }
                    }
                }
            },
            options: {
                scales: {
                    y: {
                        beginAtZero: true
                    }
                },
                responsive: true,
                plugins: {
                    legend: {
                        display: true,
                        position: 'top',
                    },
                    title: {
                        display: true,
                        text: 'Branch Wise Sales',
                        color: '#714b67',
                        font: {
                            size: 12,
                            weight: 'bold'
                        }
                    }
                }
            }
        });

        const labelBrandSalesChartItem = data.brand_wise_sales_Items.map(item => item.name);
        const amountBrandSalesChartItem = data.brand_wise_sales_Items.map(item => item.amt);
        const quantitieBrandSalesChartItem = data.brand_wise_sales_Items.map(item => item.qty);
        const ctxBrandSalesChartItem = document.getElementById('brandSalesChartItem').getContext('2d');
        const BrandSalesChartItem = new Chart(ctxBrandSalesChartItem, {
            type: 'line',
            data: {
                labels: labelBrandSalesChartItem,
                datasets: [
                    {
                        label: 'Sales Amount',
                        data: amountBrandSalesChartItem,
                        borderColor: 'rgba(75, 192, 192, 1)',
                        backgroundColor: 'rgba(75, 192, 192, 0.2)',
                        fill: true,
                        borderWidth: 2,
                        tension: 0.4,
                    },
                    {
                        label: 'Quantity Sold',
                        data: quantitieBrandSalesChartItem,
                        borderColor: 'rgba(153, 102, 255, 1)',
                        backgroundColor: 'rgba(153, 102, 255, 0.2)',
                        fill: true,
                        borderWidth: 2,
                        tension: 0.4,
                    }
                ]
            },
            options: {
                scales: {
                    y: {
                        beginAtZero: true
                    }
                },
                responsive: true,
                plugins: {
                    legend: {
                        display: true,
                        position: 'top',
                    },
                    title: {
                        display: true,
                        text: 'Brand Wise Sales - Line Chart',
                        color: '#714b67',
                    }
                }
            }
        });


        const labelCatSalesChart = data.cat_wise_sales.details.map(item => item.name);
        const amountCatSalesChart = data.cat_wise_sales.details.map(item => item.amt);
        const quantitieCatSalesChart = data.cat_wise_sales.details.map(item => item.qty);
        const ctxCatSalesChart = document.getElementById('catSalesChart').getContext('2d');
        const CatSalesCharts = new Chart(ctxCatSalesChart, {
            type: 'line',
            data: {
                labels: labelCatSalesChart,
                datasets: [
                    {
                        label: 'Sales Amount',
                        data: amountCatSalesChart,
                        backgroundColor: 'rgba(75, 192, 192, 0.4)',
                        borderColor: 'rgba(75, 192, 192, 1)',
                        borderWidth: 1,
                        fill: true,
                        tension: 0.4,
                    },
                    {
                        label: 'Quantity Sold',
                        data: quantitieCatSalesChart,
                        backgroundColor: 'rgba(153, 102, 255, 0.4)',
                        borderColor: 'rgba(153, 102, 255, 1)',
                        borderWidth: 1,
                        fill: true,
                        tension: 0.4,
                    }
                ]
            },
            options: {
                scales: {
                    y: {
                        beginAtZero: true
                    }
                },
                responsive: true,
                plugins: {
                    legend: {
                        display: true,
                        position: 'top',
                    },
                    title: {
                        display: true,
                        text: 'Category Wise Sales',
                        color: '#714b67',
                    }
                }
            }
        });


        const labelCatSalesChartItem = data.cat_wise_sales_Items.map(item => item.name);
        const amountCatSalesChartItem = data.cat_wise_sales_Items.map(item => item.amt);
        const ctxCatSalesChartItem = document.getElementById('catSalesChartItem').getContext('2d');
        const catSalesChartItems = new Chart(ctxCatSalesChartItem, {
            type: 'bar',
            data: {
                labels: labelCatSalesChartItem,
                datasets: [
                    {
                        label: 'Sales Amount',
                        data: amountCatSalesChartItem,
                        backgroundColor: [
                            'rgba(75, 192, 192, 0.6)',
                            'rgba(153, 102, 255, 0.6)',
                            'rgba(255, 159, 64, 0.6)',
                            'rgba(255, 205, 86, 0.6)',
                            'rgba(54, 162, 235, 0.6)',
                        ],
                        borderColor: [
                            'rgba(75, 192, 192, 1)',
                            'rgba(153, 102, 255, 1)',
                            'rgba(255, 159, 64, 1)',
                            'rgba(255, 205, 86, 1)',
                            'rgba(54, 162, 235, 1)',
                        ],
                        borderWidth: 1,
                    }
                ]
            },
            options: {
                responsive: true,
                plugins: {
                    legend: {
                        display: true,
                        position: 'top',
                    },
                    title: {
                        display: true,
                        text: 'Category Wise Sales (Bar Chart)', // Updated title
                    }
                },
                scales: {
                    y: {
                        beginAtZero: true,
                        title: {
                            display: true,
                            text: 'Sales Amount',
                            color: '#714b67',
                        }
                    }
                }
            }
        });


        const monthlySalesDatas = data.com_monthly_wise_sales[0].detail.details;
        const labelMonthlySalesChart = monthlySalesDatas.map(item => item.month);
        const amountMonthlySalesChart = monthlySalesDatas.map(item => item.amt);
        const quantitieMonthlySalesChart = monthlySalesDatas.map(item => item.qty);
        const ctxMonthlySalesChart = document.getElementById('monthlySalesChart').getContext('2d');
        const monthlySalesChartDatas = new Chart(ctxMonthlySalesChart, {
            type: 'bar',
            data: {
                labels: labelMonthlySalesChart,
                datasets: [
                    {
                        label: 'Sales Amount',
                        data: amountMonthlySalesChart,
                        backgroundColor: 'rgba(75, 192, 192, 0.6)',
                        borderColor: 'rgba(75, 192, 192, 1)',
                        borderWidth: 1,
                        stack: 'combined',
                    },
                    {
                        label: 'Quantity Sold',
                        data: quantitieMonthlySalesChart,
                        backgroundColor: 'rgba(153, 102, 255, 0.6)',
                        borderColor: 'rgba(153, 102, 255, 1)',
                        borderWidth: 1,
                        stack: 'combined',
                    }
                ]
            },
            options: {
                scales: {
                    y: {
                        beginAtZero: true,
                        stacked: true,
                    },
                    x: {
                        stacked: true,
                    }
                },
                responsive: true,
                plugins: {
                    legend: {
                        display: true,
                        position: 'top',
                    },
                    title: {
                        display: true,
                        text: 'Monthly Sales Data',
                        color: '#714b67',
                    }
                }
            }
        });

        // Sales Items Bar Chart
        const salesItemsDetails = data.com_wise_sales_Items;
        const labelSalesItems = salesItemsDetails.map(item => item.name);
        const amountSalesItem = salesItemsDetails.map(item => item.amt);
        const quantitieSalesItem = salesItemsDetails.map(item => item.qty);
        const ctxSalesChartItems = document.getElementById('salesChartItems').getContext('2d');
        new Chart(ctxSalesChartItems, {
            type: 'bar',
            data: {
                labels: labelSalesItems,
                datasets: [
                    {
                        label: 'Sales Amount',
                        data: amountSalesItem,
                        backgroundColor: 'rgba(75, 192, 192, 0.6)',
                        borderColor: 'rgba(75, 192, 192, 1)',
                        borderWidth: 1,
                        yAxisID: 'y'
                    },
                    {
                        label: 'Quantity Sold',
                        data: quantitieSalesItem,
                        backgroundColor: 'rgba(255, 99, 132, 0.6)',
                        borderColor: 'rgba(255, 99, 132, 1)',
                        borderWidth: 1,
                        yAxisID: 'y1'
                    }
                ]
            },
            options: {
                scales: {
                    y: {
                        beginAtZero: true,
                        title: {
                            display: true,
                            text: 'Sales Amount'
                        }
                    },
                    y1: {
                        beginAtZero: true,
                        position: 'right',
                        title: {
                            display: true,
                            text: 'Quantity Sold'
                        },
                        grid: {
                            drawOnChartArea: false
                        }
                    }
                },
                responsive: true,
                plugins: {
                    legend: {
                        display: true,
                        position: 'top',
                    },
                    title: {
                        display: true,
                        text: 'Sales Items Data',
                        color: '#714b67',
                    }
                }
            }
        });

        // Top 10 Product Sales Bar Chart
        const productSalesDetails = data.top_ten_product_sales.details;
        const labelProductSalesDetails = productSalesDetails.map(item => item.name);
        const amountProductSalesDetails = productSalesDetails.map(item => item.amt);
        const quantitieProductSalesDetails = productSalesDetails.map(item => item.qty);
        const ctxProductSalesDetails = document.getElementById('productSalesChart').getContext('2d');
        new Chart(ctxProductSalesDetails, {
            type: 'bar',
            data: {
                labels: labelProductSalesDetails,
                datasets: [
                    {
                        label: 'Sales Amount',
                        data: amountProductSalesDetails,
                        backgroundColor: 'rgba(75, 192, 192, 0.6)',
                        borderColor: 'rgba(75, 192, 192, 1)',
                        borderWidth: 1,
                        yAxisID: 'y'
                    },
                    {
                        label: 'Quantity Sold',
                        data: quantitieProductSalesDetails,
                        backgroundColor: 'rgba(255, 99, 132, 0.6)',
                        borderColor: 'rgba(255, 99, 132, 1)',
                        borderWidth: 1,
                        yAxisID: 'y1'
                    }
                ]
            },
            options: {
                scales: {
                    y: {
                        beginAtZero: true,
                        title: {
                            display: true,
                            text: 'Sales Amount'
                        }
                    },
                    y1: {
                        beginAtZero: true,
                        position: 'right',
                        title: {
                            display: true,
                            text: 'Quantity Sold'
                        },
                        grid: {
                            drawOnChartArea: false
                        }
                    }
                },
                responsive: true,
                plugins: {
                    legend: {
                        display: true,
                        position: 'top',
                    },
                    title: {
                        display: true,
                        text: 'Top 10 Product Sales Data',
                        color: '#714b67',
                    }
                }
            }
        });
        const labelsSalesChartItem = data.branch_wise_sales_Items.map(item => item.name);
        const amountSalesChartItem = data.branch_wise_sales_Items.map(item => item.amt);
        const ctxSalesChartItem = document.getElementById('salesChartItem').getContext('2d');
        const salesChartItem = new Chart(ctxSalesChartItem, {
            type: 'doughnut',
            data: {
                labels: labelsSalesChartItem,
                datasets: [
                    {
                        label: 'Sales Amount',
                        data: amountSalesChartItem,
                        backgroundColor: [
                            'rgba(75, 192, 192, 0.6)',
                            'rgba(153, 102, 255, 0.6)',
                            'rgba(255, 159, 64, 0.6)',
                            'rgba(255, 99, 132, 0.6)',
                            'rgba(54, 162, 235, 0.6)',
                            'rgba(255, 206, 86, 0.6)',
                            'rgba(75, 192, 192, 0.6)',
                            'rgba(153, 102, 255, 0.6)',
                            'rgba(255, 159, 64, 0.6)',
                            'rgba(54, 162, 235, 0.6)',
                        ],
                        borderColor: [
                            'rgba(75, 192, 192, 1)',
                            'rgba(153, 102, 255, 1)',
                            'rgba(255, 159, 64, 1)',
                            'rgba(255, 99, 132, 1)',
                            'rgba(54, 162, 235, 1)',
                            'rgba(255, 206, 86, 1)',
                            'rgba(75, 192, 192, 1)',
                            'rgba(153, 102, 255, 1)',
                            'rgba(255, 159, 64, 1)',
                            'rgba(54, 162, 235, 1)',
                        ],
                        borderWidth: 1
                    }
                ]
            },
            options: {
                responsive: true,
                plugins: {
                    legend: {
                        display: true,
                        position: 'top',
                    },
                    title: {
                        display: true,
                        text: 'Branch Wise Sales - Sales Amount',
                        color: '#714b67',
                    }
                }
            }
        });

        const labelBrandSalesChart = data.brand_wise_sales.details.map(item => item.name);
        const amountBrandSalesChart = data.brand_wise_sales.details.map(item => item.amt);
        const ctxBrandSalesChart = document.getElementById('brandSalesChart').getContext('2d');
        const brandSalesChart = new Chart(ctxBrandSalesChart, {
            type: 'pie',
            data: {
                labels: labelBrandSalesChart,
                datasets: [
                    {
                        label: 'Sales Amount',
                        data: amountBrandSalesChart,
                        backgroundColor: [
                            'rgba(75, 192, 192, 0.6)',
                            'rgba(153, 102, 255, 0.6)',
                            'rgba(255, 159, 64, 0.6)',
                            'rgba(255, 99, 132, 0.6)',
                            'rgba(54, 162, 235, 0.6)',
                            'rgba(255, 206, 86, 0.6)'
                        ],
                        borderColor: [
                            'rgba(75, 192, 192, 1)',
                            'rgba(153, 102, 255, 1)',
                            'rgba(255, 159, 64, 1)',
                            'rgba(255, 99, 132, 1)',
                            'rgba(54, 162, 235, 1)',
                            'rgba(255, 206, 86, 1)'
                        ],
                        borderWidth: 1,
                    }
                ]
            },
            options: {
                responsive: true,
                plugins: {
                    legend: {
                        display: true,
                        position: 'top',
                    },
                    title: {
                        display: true,
                        text: 'Brand Wise Sales - Sales Amount',
                        color: '#714b67',
                    }
                }
            }
        });

        //Prem Prakash Dhakal
        const labelProductSalesItem = data.top_ten_product_sales_Items.map(item => item.name);
        const amountProductSalesItem = data.top_ten_product_sales_Items.map(item => item.amt);
        const quantitieProductSalesItem = data.top_ten_product_sales_Items.map(item => item.qty);
        const ctxProductSalesItem = document.getElementById('productSalesChartItem').getContext('2d');
        const productSalesChart = new Chart(ctxProductSalesItem, {
            type: 'bar',
            data: {
                labels: labelProductSalesItem,
                datasets: [
                    {
                        label: 'Sales Amount',
                        data: amountProductSalesItem,
                        backgroundColor: 'rgba(75, 192, 192, 0.6)',
                        borderColor: 'rgba(75, 192, 192, 1)',
                        borderWidth: 1,
                        yAxisID: 'y'
                    },
                    {
                        label: 'Quantity Sold',
                        data: quantitieProductSalesItem,
                        backgroundColor: 'rgba(255, 99, 132, 0.6)',
                        borderColor: 'rgba(255, 99, 132, 1)',
                        borderWidth: 1,
                        yAxisID: 'y1'
                    }
                ]
            },
            options: {
                scales: {
                    y: {
                        beginAtZero: true,
                        title: {
                            display: true,
                            text: 'Sales Amount'
                        }
                    },
                    y1: {
                        beginAtZero: true,
                        position: 'right',
                        title: {
                            display: true,
                            text: 'Quantity Sold'
                        },
                        grid: {
                            drawOnChartArea: false
                        }
                    }
                },
                responsive: true,
                plugins: {
                    legend: {
                        display: true,
                        position: 'top',
                    },
                    title: {
                        display: true,
                        text: 'Top Ten Product Sales',
                        color: '#714b67',
                    }
                }
            }
        });
        const chartData = data.top_ten_product_sales_Items.map(item => ({
            product: item.name,
            value: item.amt
        }));
        //create3DPieChart(chartData);
        //create3DFunnelChart(chartData);
        //create3DConeChart(chartData);
        //create3DCylinderChart(chartData);
    }
            catch (error) {
    console.error('Error fetching or rendering the charts:', error.message, error.stack);
}
        }
//function create3DPieChart(chartData) {
//    am4core.ready(function () {
//        am4core.useTheme(am4themes_animated);
//        var chart = am4core.create("chartdiv", am4charts.PieChart3D);
//        chart.data = chartData;

//        var pieSeries = chart.series.push(new am4charts.PieSeries3D());
//        pieSeries.dataFields.value = "value";
//        pieSeries.dataFields.category = "product";
//        pieSeries.slices.template.stroke = am4core.color("#fff");
//        pieSeries.slices.template.strokeWidth = 2;
//        pieSeries.slices.template.strokeOpacity = 1;
//        chart.innerRadius = am4core.percent(40);
//        pieSeries.depth = 20;
//        pieSeries.angle = 30;

//        pieSeries.labels.template.fontSize = 12;
//        pieSeries.labels.template.fill = am4core.color("#000");
//        pieSeries.labels.template.fontWeight = "bold";

//        pieSeries.ticks.template.strokeWidth = 2;
//        pieSeries.ticks.template.length = 20;
//    });
//}

//function create3DFunnelChart(chartData) {
//    var funnelData = [];
//    for (var i = 0; i < chartData.length; i++) {
//        funnelData.push({
//            "title": chartData[i].product,
//            "value": chartData[i].value
//        });
//    }

//    var chart = AmCharts.makeChart("chartFunneldiv", {
//        "type": "funnel",
//        "dataProvider": funnelData,
//        "balloon": {
//            "fixedPosition": true
//        },
//        "legend": { },
//        "valueField": "value",
//        "titleField": "title",
//        "marginRight": 240,
//        "marginLeft": 50,
//        "startX": -500,
//        "depth3D": 100,
//        "angle": 40,
//        "outlineAlpha": 1,
//        "outlineColor": "#FFFFFF",
//        "outlineThickness": 2,
//        "labelPosition": "right",
//        "balloonText": "[[title]]: [[value]] units sold",
//        "responsive": {
//            "enabled": true
//        }
//    });
//}

//var chartData1 = [], chartData2 = [], chartData3 = [], chartData4 = [], chartData5 = [], chartData6 = [], chartData7 = [];
//generateChartData();

//function generateChartData() {
//    var firstDate = new Date();
//    firstDate.setDate(firstDate.getDate() - 500);
//    firstDate.setHours(0, 0, 0, 0);

//    for (var i = 0; i < 500; i++) {
//        var newDate = new Date(firstDate);
//        newDate.setDate(newDate.getDate() + i);

//        var a1 = Math.round(Math.random() * (40 + i)) + 100 + i;
//        var a2 = Math.round(Math.random() * (100 + i)) + 200 + i;
//        var a3 = Math.round(Math.random() * (100 + i)) + 200;
//        var a4 = Math.round(Math.random() * (100 + i)) + 200 + i;
//        var a5 = Math.round(Math.random() * (50 + i)) + 150 + i;
//        var a6 = Math.round(Math.random() * (60 + i)) + 250 + i;
//        var a7 = Math.round(Math.random() * (70 + i)) + 300 + i;

//        var b1 = chartData[0].product;
//        var b2 = chartData[1].product;
//        var b3 = chartData[2].product;
//        var b4 = chartData[3].product;
//        var b5 = chartData[4].product;
//        var b6 = chartData[5].product;
//        var b7 = chartData[6].product;

//        chartData1.push({"product": b1, "value": a1 });
//        chartData2.push({"product": b2, "value": a2 });
//        chartData3.push({"product": b3, "value": a3 });
//        chartData4.push({"product": b4, "value": a4 });
//        chartData5.push({"product": b5, "value": a5 });
//        chartData6.push({"product": b6, "value": a6 });
//        chartData7.push({"product": b7, "value": a7 });
//    }
//}
//create3DFunnelChart([
//    {"value": chartData1[chartData1.length - 1].value, "product": chartData1[chartData1.length - 1].product },
//    {"value": chartData2[chartData2.length - 1].value, "product": chartData2[chartData2.length - 1].product },
//    {"value": chartData3[chartData3.length - 1].value, "product": chartData3[chartData3.length - 1].product },
//    {"value": chartData4[chartData4.length - 1].value, "product": chartData4[chartData4.length - 1].product },
//    {"value": chartData5[chartData5.length - 1].value, "product": chartData5[chartData5.length - 1].product },
//    {"value": chartData6[chartData6.length - 1].value, "product": chartData6[chartData6.length - 1].product },
//    {"value": chartData7[chartData7.length - 1].value, "product": chartData7[chartData7.length - 1].product }
//]);


//// Function to create a 3D Cone Chart
//function create3DConeChart(chartData) {
//    var coneChart = am4core.create("chart3DConeldiv", am4charts.XYChart3D);

//    coneChart.data = chartData;

//    var categoryAxis = coneChart.xAxes.push(new am4charts.CategoryAxis());
//    categoryAxis.dataFields.category = "product";
//    categoryAxis.renderer.labels.template.rotation = -45;
//    categoryAxis.renderer.labels.template.hideOversized = false;
//    categoryAxis.renderer.labels.template.truncate = true;

//    var valueAxis = coneChart.yAxes.push(new am4charts.ValueAxis());

//    var series = coneChart.series.push(new am4charts.ConeSeries());
//    series.dataFields.valueY = "value";
//    series.dataFields.categoryX = "product";
//    series.columns.template.tooltipText = "{categoryX}: {valueY}";

//    // 3D settings
//    coneChart.depth3D = 20;
//    coneChart.angle = 30;

//    // 3D shadows
//    series.columns.template.strokeOpacity = 0;
//    series.columns.template.fillOpacity = 0.7;
//}

//// Function to create a 3D Cylinder Chart
//function create3DCylinderChart(chartData) {
//    var cylinderChart = am4core.create("chart3DCylinderldiv", am4charts.XYChart3D);

//    cylinderChart.data = chartData;

//    var categoryAxis = cylinderChart.xAxes.push(new am4charts.CategoryAxis());
//    categoryAxis.dataFields.category = "product";
//    categoryAxis.renderer.labels.template.rotation = -45;
//    categoryAxis.renderer.labels.template.hideOversized = false;
//    categoryAxis.renderer.labels.template.truncate = true;

//    var valueAxis = cylinderChart.yAxes.push(new am4charts.ValueAxis());

//    var series = cylinderChart.series.push(new am4charts.CylinderSeries());
//    series.dataFields.valueY = "value";
//    series.dataFields.categoryX = "product";
//    series.columns.template.tooltipText = "{categoryX}: {valueY}";

//    // 3D settings
//    cylinderChart.depth3D = 20;
//    cylinderChart.angle = 30;

//    // 3D shadows
//    series.columns.template.strokeOpacity = 0;
//    series.columns.template.fillOpacity = 0.7;
//}

async function fetchSalesData() {
    const BASE_URL = @Html.Raw(Json.Encode(ViewBag.BaseUrlForDashboard));
    try {
        const response = await fetch(`${BASE_URL}/api/Indicatorpanel/GetGroupWiseSalesReport`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                repType: "group_wise_sales",
                branchCode: @Html.Raw(Json.Encode(ViewBag.BranchCode)),
        companyCode: @Html.Raw(Json.Encode(ViewBag.CompanyCode)),
        UserId: @Html.Raw(Json.Encode(ViewBag.UserId)),
        @* fromDate: @Html.Raw(Json.Encode(ViewBag.StartFromDate)),
        toDate: @Html.Raw(Json.Encode(ViewBag.EndToDate)),*@
        //fromDate: "17-Jul-2024",
        //toDate: "16-Jul-2025",
        fromDate: @Html.Raw(Json.Encode(ViewBag.StartFromDate)),
        toDate: @Html.Raw(Json.Encode(ViewBag.EndToDate)),
        itemStringFilter: [],
            customerCode: "",
                dealerCode: "",
                    months: ["This Month", "This Year", "Last Month"],
                        cat: "",
                            brand: "",
                                grp: "",
                                    grossNet: "",
                                        grossNetStr: "",
                                            brand: "",
                                                cusGrp: "",
                                                    cusGrpStr: "",
                                                        unitType: "",
                                                            with_eng_date: ""
    })
});

if (!response.ok) {
    throw new Error('Failed to fetch data');
}

const data = await response.json();
populateTable(data.Group_Wise_Sales);
            } catch (error) {
    console.error('Error fetching sales data:', error);
}
        }

function populateTable(salesData) {
    const tableBody = document.querySelector('#salesGrid tbody');
    tableBody.innerHTML = '';

    salesData.forEach(item => {
        const row = document.createElement('tr');
        row.setAttribute('data-group-flag', item.group_flag);
        row.innerHTML = `
        <td>${item.item}</td>
        <td>${item.sales_qty}</td>
        <td>${item.sales_amt}</td>
        <td>${item.sales_ret_qty}</td>
        <td>${item.sales_ret_value}</td>
        <td>${item.net_sales_qty}</td>
        <td>${item.net_sales_value}</td>
        `;
        row.addEventListener('click', function () {
            handleRowClick(item, row);
        });

        tableBody.appendChild(row);
    });
}

function handleRowClick(item, row) {
    const groupFlag = row.getAttribute('data-group-flag');

    if (groupFlag === 'G') {
        const childRows = findChildItems(item);
        toggleChildRows(row, childRows);
    }
}

function toggleChildRows(parentRow, childRows) {
    const tableBody = parentRow.parentElement;

    childRows.forEach(childRow => {
        if (childRow.parentRow === parentRow) {
            const existingRow = tableBody.querySelector(`#child-${childRow.id}`);
            if (existingRow) {
                existingRow.style.display = existingRow.style.display === 'none' ? '' : 'none';
            } else {
                const newRow = createChildRow(childRow);
                tableBody.insertBefore(newRow, parentRow.nextSibling);
            }
        }
    });
}
function createChildRow(childItem) {
    const row = document.createElement('tr');
    row.id = `child-${childItem.id}`;
    row.innerHTML = `
        <td colspan="7">
            <div>Child Item Data: ${childItem.item_desc} | Sales: ${childItem.sales_qty}</div>
        </td>
        `;
    return row;
}
function findChildItems(parentItem) {
    return [
        {
            id: 'child-1',
            parentRow: parentItem,
            item_desc: 'Child Item 1',
            sales_qty: 10
        },
        {
            id: 'child-2',
            parentRow: parentItem,
            item_desc: 'Child Item 2',
            sales_qty: 15
        }
    ];
}

async function loadDropdownData() {
    const BASE_URL = @Html.Raw(Json.Encode(ViewBag.BaseUrlForDashboard));
    const companyCode = @Html.Raw(Json.Encode(ViewBag.CompanyCode));
    const requestData = { "company_code": companyCode };

    try {
        const response = await fetch(`${BASE_URL}/api/Indicatorpanel/AllSubLedger`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(requestData)
        });
        if (!response.ok) throw new Error("Network response was not ok");

        const data = await response.json();
        populateDropdown("repCategoryWise", data.cat);
        populateDropdown("repCustomerWise", data.cus_grp);
        populateDropdown("repGroupWise", data.grp);
        populateDropdown("repBrandWise", data.brand, true);
        populateDropdown("repItemWise", data.grp);
        populateDropdown("repDealerWise", data.sup_grp);
    } catch (error) {
        console.error("Error fetching data:", error);
    }
}

function populateDropdown(selectId, items, isList = false) {
    const selectElement = document.getElementById(selectId);
    selectElement.innerHTML = '';
    if (isList) {
        items.forEach(item => {
            const option = document.createElement("option");
            option.value = item;
            option.text = item;
            selectElement.appendChild(option);
        });
    } else {
        for (const [key, value] of Object.entries(items)) {
            const option = document.createElement("option");
            option.value = value;
            option.text = key;
            selectElement.appendChild(option);
        }
    }
}
async function performSearch() {
    try {
        const BASE_URL = @Html.Raw(Json.Encode(ViewBag.BaseUrlForDashboard));
        const repCategoryWise = document.getElementById("repCategoryWise").value;
        const repCustomerWise = document.getElementById("repCustomerWise").value;
        const repGroupWise = document.getElementById("repGroupWise").value;
        const repBrandWise = document.getElementById("repBrandWise").value;
        const repItemWise = document.getElementById("repItemWise").value;
        const repDealerWise = document.getElementById("repDealerWise").value;
        const fromDate = document.getElementById("fromDate").value;
        const toDate = document.getElementById("toDate").value;
        const requestData = {
            repType: "graph_wise_sales",
            branchCode: @Html.Raw(Json.Encode(ViewBag.BranchCode)),
        companyCode: @Html.Raw(Json.Encode(ViewBag.CompanyCode)),
        UserId: @Html.Raw(Json.Encode(ViewBag.UserId)),
        //fromDate: fromDate,
        //toDate: toDate,
        fromDate: @Html.Raw(Json.Encode(ViewBag.StartFromDate)),
        toDate: @Html.Raw(Json.Encode(ViewBag.EndToDate)),
        itemStringFilter: [repItemWise],
            customerCode: repCustomerWise,
                dealerCode: repDealerWise,
                    months: ["This Month", "This Year", "Last Month"],
                        cat: repCategoryWise,
                            brand: repBrandWise,
                                grp: repGroupWise,
                                    grossNet: "",
                                        grossNetStr: "",
                                            brand: "",
                                                cusGrp: "",
                                                    cusGrpStr: "",
                                                        unitType: "",
                                                            with_eng_date: ""
    };

    const response = await fetch(`${BASE_URL}/api/Indicatorpanel/GetGraphWiseSalesReport`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(requestData)
    });

    if (!response.ok) throw new Error('Network response was not ok');
    const data = await response.json();
    console.log(data);

} catch (error) {
    console.error('There was a problem with the fetch operation:', error);
}
    }
function openForm() {
    document.getElementById("popupForm").style.display = "flex";
}
function closeForm() {
    document.getElementById("popupForm").style.display = "none";
}

async function fetchGraphWiseSalesReportForKendoUI() {
    const BASE_URL = ViewBag.BaseUrlForDashboard;

    try {
        const requestData = {
            repType: "graph_wise_sales",
            branchCode: @Html.Raw(Json.Encode(ViewBag.BranchCode)),
        companyCode: @Html.Raw(Json.Encode(ViewBag.CompanyCode)),
        UserId: @Html.Raw(Json.Encode(ViewBag.UserId)),
        //fromDate: "17-Jul-2024",
        //toDate: "16-Jul-2025",
        fromDate: @Html.Raw(Json.Encode(ViewBag.StartFromDate)),
        toDate: @Html.Raw(Json.Encode(ViewBag.EndToDate)),
        months: [reportType]
    };

    const response = await fetch(`${BASE_URL}/api/Indicatorpanel/GetGraphWiseSalesReport`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(requestData)
    });

    const data = await response.json();
    renderSalesChart(data.branch_wise_sales.details);
} catch (error) {
    console.error("Error fetching data:", error.message);
}
}

function renderSalesChart(details) {
    const labels = details.map(branch => branch.name);
    const salesAmount = details.map(branch => branch.amt);
    const salesQuantity = details.map(branch => branch.qty);

    $("#salesChart").kendoChart({
        title: {
            text: "Branch Wise Sales"
        },
        legend: {
            position: "top"
        },
        series: [
            { name: "Sales Amount", data: salesAmount, type: "column", color: "#3A7BD5" },
            { name: "Quantity Sold", data: salesQuantity, type: "line", color: "#FF9F40" }
        ],
        categoryAxis: {
            categories: labels,
            title: { text: "Branches" }
        },
        valueAxis: {
            labels: { format: "{0}" },
            title: { text: "Values" }
        },
        tooltip: {
            visible: true,
            format: "{0}",
            template: "#= series.name #: #= value #"
        }
    });
}

// Initialize
// fetchGraphWiseSalesReportForKendoUI();

$(document).ready(function () {
    // Collapse Functionality
    $(".collapse").click(function () {
        const content = $(this).closest(".dashboard-widget").find(".widget-content");
        content.toggle();
        $(this).text(content.is(":visible") ? "− Collapse" : "+ Expand");
    });

    // Fullscreen Mode
    $(".fullscreen").click(function () {
        const widget = $(this).closest(".dashboard-widget");
        widget.toggleClass("fullscreen-mode");
        $(this).text(widget.hasClass("fullscreen-mode") ? "⤢ Exit Fullscreen" : "⛶ Fullscreen");
    });

    // Remove Widget
    $(".remove").click(function () {
        $(this).closest(".dashboard-widget").remove();
    });

    // Global Controls
    $("#expand-all").click(function () {
        $(".widget-content").show();
        $(".collapse").text("− Collapse");
    });

    $("#reset-widgets").click(function () {
        location.reload();
    });

    $("#reload-widgets").click(function () {
        fetchGraphWiseSalesReportForKendoUI();
    });
});

let ctxChart = null;
let monthlySalesChartInstance;
let reportType;
let reportTypePeriodic = "This Month";
document.querySelectorAll('.dropdown-item').forEach(item => {
    item.addEventListener('click', function (e) {
        reportType = e.target.getAttribute('data-report-type');
        updateChart(reportType);
    });
});
function updateChart(reportType) {
    let chartType;
    switch (reportType) {
        case 'LineChart':
            chartType = 'line';
            break;
        case 'PieChart':
            chartType = 'pie';
            break;
        case 'ColumnChart':
            chartType = 'column';
            break;
        case 'TableView':
            chartType = 'table';
            break;
        case 'PivotTable':
            chartType = 'pivot';
            break;
        default:
            chartType = 'line';
    }
    fetchLineChartRender(chartType, reportTypePeriodic);
}

async function fetchLineChartRender(chartType, reportTypePeriodic) {
    const ChartBASE_URL = @Html.Raw(Json.Encode(ViewBag.BaseUrlForDashboard));
    try {
        const requestData = {
            repType: "graph_wise_sales",
            branchCode: @Html.Raw(Json.Encode(ViewBag.BranchCode)),
        companyCode: @Html.Raw(Json.Encode(ViewBag.CompanyCode)),
        UserId: @Html.Raw(Json.Encode(ViewBag.UserId)),
        //fromDate: "17-Jul-2023",
        //toDate: "16-Jul-2024",
        fromDate: @Html.Raw(Json.Encode(ViewBag.StartFromDate)),
        toDate: @Html.Raw(Json.Encode(ViewBag.EndToDate)),
        itemStringFilter: [],
            customerCode: "",
                dealerCode: "",
                    months: ["This Month", "This Year", "Last Month"],
                        cat: "",
                            brand: "",
                                grp: "",
                                    grossNet: "",
                                        grossNetStr: "",
                                            cusGrp: "",
                                                cusGrpStr: "",
                                                    unitType: "",
                                                        with_eng_date: ""
    };

    const response = await fetch(`${ChartBASE_URL}/api/Indicatorpanel/GetGraphWiseSalesReport`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(requestData)
    });

    const data = await response.json();
    const labelSalesChart = data.com_wise_sales.details.map(branch => branch.name);
    const amountSalesChart = data.com_wise_sales.details.map(branch => branch.amt);
    const quantitieSalesChart = data.com_wise_sales.details.map(branch => branch.qty);

    switch (chartType) {
        case 'line':
            renderLineChart(labelSalesChart, amountSalesChart, quantitieSalesChart);
            break;
        case 'bar':
            renderBarChart(labelSalesChart, amountSalesChart);
            break;
        case 'pie':
            renderPieChart(labelSalesChart, amountSalesChart);
            break;
        case 'table':
            renderTableView(data.com_wise_sales.details);
            break;
        case 'column':
            renderColumnChart(labelSalesChart, amountSalesChart, quantitieSalesChart);
            break;
        case 'pivot':
            renderPivotTable(data.com_wise_sales.details);
            break;
        default:
            console.error('Unknown chart type');
    }
} catch (error) {
    console.error("Error fetching sales data:", error);
}
            }

function renderLineChart(labelSalesChart, amountSalesChart, quantitieSalesChart) {
    if (ctxChart) {
        ctxChart.destroy();
    }
    const ctxmonthlySalesChart = document.getElementById('monthlySalesLineChart').getContext('2d');
    ctxChart = new Chart(ctxmonthlySalesChart, {
        type: 'line',
        data: {
            labels: labelSalesChart,
            datasets: [
                {
                    label: 'Sales Amount',
                    data: amountSalesChart,
                    borderColor: 'rgba(58, 123, 213, 1)',
                    backgroundColor: 'rgba(58, 123, 213, 0.2)',
                    fill: true,
                    borderWidth: 2,
                    tension: 0.4
                },
                {
                    label: 'Quantity Sold',
                    data: quantitieSalesChart,
                    borderColor: 'rgba(255, 159, 64, 1)',
                    backgroundColor: 'rgba(255, 159, 64, 0.2)',
                    fill: true,
                    borderWidth: 2,
                    tension: 0.4
                }
            ]
        },
        options: {
            scales: {
                x: {
                    grid: { color: 'rgba(255, 255, 255, 0.1)' },
                    ticks: { color: 'white' }
                },
                y: {
                    grid: { color: 'rgba(255, 255, 255, 0.1)' },
                    ticks: { color: 'white' }
                }
            },
            plugins: {
                legend: { labels: { color: 'white' } },
                tooltip: {
                    backgroundColor: 'rgba(0, 0, 0, 0.7)',
                    titleColor: 'white',
                    bodyColor: 'white',
                    borderColor: 'rgba(75, 192, 192, 1)',
                    borderWidth: 1
                },
                title: {
                    display: true,
                    text: 'Branch Wise Sales',
                    color: '#714b67',
                    font: { size: 12, weight: 'bold' }
                }
            },
            responsive: true
        }
    });
    toggleChartVisibility('line');
}

function renderBarChart(labels, amounts) {
    if (ctxChart) {
        ctxChart.destroy();
    }
    ctxMonthlySalesBarChart = document.getElementById('monthlySalesBarChart').getContext('2d');
    ctxChart = new Chart(ctxMonthlySalesBarChart, {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [
                {
                    label: 'Amount',
                    data: amounts,
                    backgroundColor: 'rgba(54, 162, 235, 0.6)',
                    borderColor: 'rgba(54, 162, 235, 1)',
                    borderWidth: 1,
                },
            ],
        },
        options: {
            indexAxis: 'y',
        },
    });
}

//    function renderPieChart(labelSalesChart, amountSalesChart) {
//        if (ctxChart) {
//            ctxChart.destroy();
//        }
//        const salesChart = document.getElementById('monthlySalesPieChart').getContext('2d');
//        ctxChart = new Chart(salesChart, {
//        type: 'pie',
//        data: {
//            labels: labelSalesChart,
//            datasets: [{
//                label: 'Sales Amount',
//                data: amountSalesChart,
//                backgroundColor: ['rgba(255, 99, 132, 0.2)', 'rgba(54, 162, 235, 0.2)', 'rgba(255, 206, 86, 0.2)', 'rgba(75, 192, 192, 0.2)'],
//                borderColor: ['rgba(255, 99, 132, 1)', 'rgba(54, 162, 235, 1)', 'rgba(255, 206, 86, 1)', 'rgba(75, 192, 192, 1)'],
//                borderWidth: 1
//            }]
//        },
//        options: {
//            plugins: {
//                legend: { display: true, position: 'top', labels: { color: 'white' } },
//                title: {
//                    display: true,
//                    text: `Pie Chart - ${reportTypePeriodic || ''}`,
//                    color: 'white',
//                    font: { size: 14, weight: 'bold' }
//                },
//                tooltip: {
//                    backgroundColor: 'rgba(0, 0, 0, 0.7)',
//                    titleColor: 'white',
//                    bodyColor: 'white'
//                }
//            },
//            responsive: true
//        }
//    });
//    toggleChartVisibility('pie');
//}

//function renderPieChart(labelSalesChart, amountSalesChart) {
//    if (ctxChart) {
//        ctxChart.destroy();
//    }
//    const salesChartCanvas = document.getElementById('monthlySalesPieChart');
//    salesChartCanvas.width = 400;
//    salesChartCanvas.height = 300;
//    const ctx = salesChartCanvas.getContext('2d');
//    ctxChart = new Chart(ctx, {
//        type: 'pie',
//        data: {
//            labels: labelSalesChart,
//            datasets: [{
//                label: 'Sales Amount',
//                data: amountSalesChart,
//                backgroundColor: [
//                    'rgba(255, 99, 132, 0.2)',
//                    'rgba(54, 162, 235, 0.2)',
//                    'rgba(255, 206, 86, 0.2)',
//                    'rgba(75, 192, 192, 0.2)'
//                ],
//                borderColor: [
//                    'rgba(255, 99, 132, 1)',
//                    'rgba(54, 162, 235, 1)',
//                    'rgba(255, 206, 86, 1)',
//                    'rgba(75, 192, 192, 1)'
//                ],
//                borderWidth: 1
//            }]
//        },
//        options: {
//            plugins: {
//                legend: {
//                    display: true,
//                    position: 'top',
//                    labels: {
//                        color: 'white'
//                    }
//                },
//                title: {
//                    display: true,
//                    text: `Pie Chart - ${reportTypePeriodic || ''}`,
//                    color: 'white',
//                    font: {
//                        size: 14,
//                        weight: 'bold'
//                    }
//                },
//                tooltip: {
//                    backgroundColor: 'rgba(0, 0, 0, 0.7)',
//                    titleColor: 'white',
//                    bodyColor: 'white'
//                }
//            },
//            responsive: true
//        }
//    });

//    toggleChartVisibility('pie');
//}


/*    Chart.register(ChartDataLabels);*/
function renderPieChart(labelSalesChart, amountSalesChart) {
    if (ctxChart) {
        ctxChart.destroy();
    }

    const salesChartCanvas = document.getElementById('monthlySalesPieChart');
    salesChartCanvas.width = 400;
    salesChartCanvas.height = 300;

    const ctx = salesChartCanvas.getContext('2d');

    const colors = [
        'rgba(63, 81, 181, 0.7)', // Blue
        'rgba(33, 150, 243, 0.7)', // Light Blue
        'rgba(76, 175, 80, 0.7)',  // Green
        'rgba(255, 193, 7, 0.7)',  // Yellow
        'rgba(156, 39, 176, 0.7)', // Purple
        'rgba(244, 67, 54, 0.7)',  // Red
        'rgba(255, 87, 34, 0.7)',  // Orange
        'rgba(121, 85, 72, 0.7)'   // Brown
    ];

    ctxChart = new Chart(ctx, {
        type: 'pie',
        data: {
            labels: labelSalesChart,
            datasets: [{
                label: 'Sales Amount',
                data: amountSalesChart,
                backgroundColor: colors,
                borderColor: colors.map(color => color.replace('0.7', '1')),
                borderWidth: 1
            }]
        },
        options: {
            plugins: {
                legend: {
                    display: true,
                    position: 'top',
                    labels: {
                        color: '#333',
                        font: {
                            size: 12
                        }
                    }
                },
                title: {
                    display: true,
                    text: `Pie Chart - ${reportTypePeriodic || ''}`,
                    color: '#333',
                    font: {
                        size: 14,
                        weight: 'bold'
                    }
                },
                tooltip: {
                    backgroundColor: 'rgba(0, 0, 0, 0.7)',
                    titleColor: 'white',
                    bodyColor: 'white'
                },
                datalabels: {
                    display: true,
                    color: 'white',
                    formatter: (value, context) => {
                        const total = context.chart._metasets[0].total;
                        const percentage = ((value / total) * 100).toFixed(2) + '%';
                        return `${value} (${percentage})`;
                    },
                    font: {
                        weight: 'bold',
                        size: 12
                    },
                    anchor: 'center',
                    align: 'center'
                }
            },
            responsive: true
        }
    });

    toggleChartVisibility('pie');
}
//    function renderColumnChart(labels, amounts, quantities) {
//        if (ctxChart) {
//            ctxChart.destroy();
//        }
//        const currentChart= document.getElementById('monthlySalesColumnChart').getContext('2d');
//        ctxChart = new Chart(currentChart, {
//        type: 'bar',
//        data: {
//            labels: labels,
//            datasets: [
//                {
//                    label: 'Amount',
//                    data: amounts,
//                    backgroundColor: 'rgba(75, 192, 192, 0.6)',
//                    borderColor: 'rgba(75, 192, 192, 1)',
//                    borderWidth: 1,
//                },
//                {
//                    label: 'Quantity',
//                    data: quantities,
//                    backgroundColor: 'rgba(255, 99, 132, 0.6)',
//                    borderColor: 'rgba(255, 99, 132, 1)',
//                    borderWidth: 1,
//                },
//            ],
//        },
//    });
//}


/* Chart.register(ChartDataLabels);*/
function renderColumnChart(labels, amounts, quantities) {
    if (ctxChart) {
        ctxChart.destroy();
    }

    const currentChart = document.getElementById('monthlySalesColumnChart').getContext('2d');
    ctxChart = new Chart(currentChart, {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [
                {
                    label: 'Amount',
                    data: amounts,
                    backgroundColor: 'rgba(33, 150, 243, 0.6)',
                    borderColor: 'rgba(33, 150, 243, 1)',
                    borderWidth: 1,
                },
                {
                    label: 'Quantity',
                    data: quantities,
                    backgroundColor: 'rgba(76, 175, 80, 0.6)',
                    borderColor: 'rgba(76, 175, 80, 1)',
                    borderWidth: 1,
                }
            ]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    display: true,
                    position: 'top',
                    labels: {
                        color: '#333',
                        font: {
                            size: 12
                        }
                    }
                },
                title: {
                    display: true,
                    text: 'Sales Data - Amount & Quantity',
                    color: '#333',
                    font: {
                        size: 14,
                        weight: 'bold'
                    }
                },
                tooltip: {
                    backgroundColor: 'rgba(0, 0, 0, 0.7)',
                    titleColor: 'white',
                    bodyColor: 'white'
                },
                datalabels: {
                    display: true,
                    color: 'white',
                    formatter: (value, context) => {
                        return `${value}`;
                    },
                    font: {
                        weight: 'bold',
                        size: 12
                    },
                    anchor: 'center',
                    align: 'center'
                }
            },
            scales: {
                x: {
                    ticks: {
                        color: '#333',
                    }
                },
                y: {
                    ticks: {
                        color: '#333',
                        beginAtZero: true,
                    }
                }
            },
            barPercentage: 0.8,
            categoryPercentage: 0.7,
        }
    });
}



function renderTableView(details) {
    const tbody = document.getElementById('salesTableBody');
    if (!tbody) {
        console.error('Table body element with id "salesTableBody" not found.');
        return;
    }
    tbody.innerHTML = '';
    if (details && details.length > 0) {
        details.forEach((branch, index) => {
            const row = document.createElement('tr');
            row.className = index % 2 === 0 ? 'even-row' : 'odd-row';
            const formattedAmount = new Intl.NumberFormat('en-NP', {
                style: 'currency',
                currency: 'NPR',
                currencyDisplay: 'code',
            }).format(branch.amt);
            const customFormattedAmount = formattedAmount.replace('NPR', 'Rs.');

            row.innerHTML = `
                <td style="width:500px">${branch.name}</td>
                <td style="width:230px" class="text-right">${customFormattedAmount}</td>
                <td style="width:230px" class="text-right">${branch.qty}</td>
            `;
            tbody.appendChild(row);
        });
    } else {

        const row = document.createElement('tr');
        row.innerHTML = `<td colspan="3" class="no-data">No data available</td>`;
        tbody.appendChild(row);
    }
    toggleChartVisibility('table');
}

function renderPivotTable(details) {
    const tbody = document.getElementById('salesPivotTableBody');
    if (!tbody) {
        console.error('Table body element with id "salesPivotTableBody" not found.');
        return;
    }
    tbody.innerHTML = '';

    if (details && details.length > 0) {
        const pivotData = details.reduce((acc, branch) => {
            if (!acc[branch.name]) {
                acc[branch.name] = { name: branch.name, totalAmt: 0, totalQty: 0 };
            }
            acc[branch.name].totalAmt += branch.amt;
            acc[branch.name].totalQty += branch.qty;
            return acc;
        }, {});

        const groupedDetails = Object.values(pivotData);

        groupedDetails.forEach((group, index) => {
            const row = document.createElement('tr');
            row.className = index % 2 === 0 ? 'even-row' : 'odd-row';

            const formattedAmount = new Intl.NumberFormat('en-NP', {
                style: 'currency',
                currency: 'NPR',
                currencyDisplay: 'code',
            }).format(branch.amt);
            const customFormattedAmount = formattedAmount.replace('NPR', 'Rs.');

            row.innerHTML = `
        <td style="width:500px">${group.name}</td>
        <td style="width:240px" class="text-right">${customFormattedAmount}</td>
        <td style="width:230px" class="text-right">${group.totalQty}</td>
        `;
            tbody.appendChild(row);
        });
    } else {
        const row = document.createElement('tr');
        row.innerHTML = `<td colspan="3" class="no-data">No data available</td>`;
        tbody.appendChild(row);
    }
    const table = document.getElementById('salesPivotTable');
    if (table) {
        table.style.display = 'table';
    }
}

function clearCanvas() {
    if (ctxmonthlySalesChart) {
        ctxmonthlySalesChart.clearRect(0, 0, ctxmonthlySalesChart.canvas.width, ctxmonthlySalesChart.canvas.height);
    }
}

function clearCanvas(chartId) {
    const canvas = document.getElementById(chartId);
    if (canvas && monthlySalesChartInstance) {
        monthlySalesChartInstance.destroy();
        monthlySalesChartInstance = null;
    }
}

function toggleChartVisibility(chartType) {
    const charts = ['monthlySalesChart', 'monthlySalesLineChart', 'monthlySalesPieChart', 'monthlySalesBarChart', 'monthlySalesColumnChart', 'salesTable', 'salesPivotTable'];
    charts.forEach(chart => {
        document.getElementById(chart).style.display = 'none';
    });

    switch (chartType) {
        case 'line':
            document.getElementById('monthlySalesLineChart').style.display = 'block';
            break;
        case 'pie':
            document.getElementById('monthlySalesPieChart').style.display = 'block';
            break;
        case 'bar':
            document.getElementById('monthlySalesBarChart').style.display = 'block';
            break;
        case 'table':
            document.getElementById('salesTable').style.display = 'block';
            break;
        case 'column':
            document.getElementById('monthlySalesColumnChart').style.display = 'block';
            break;
        case 'pivot':
            document.getElementById('salesPivotTable').style.display = 'block';
            break;
        default:
            console.error('Unknown chart type for visibility toggling');
    }
}

ctxChart = null;
monthlySalesChartInstance;
reportType;
reportTypePeriodic = "This Month";

$(document).ready(function () {
    $(document).on("click", ".dropdown-item_one", function (e) {
        e.preventDefault();
        const reportType = $(this).data("report-type");
        alert(reportType);
    });
});

const Sales_BASE_URL = @Html.Raw(Json.Encode(ViewBag.BaseUrlForDashboard));
async function UpdateSalesTodayYTDMTD(reportType) {
    const apiUrl = `${Sales_BASE_URL}/api/Indicatorpanel/GetSalesTodayYTDMTD`;
    const params = {
        repType: "GetSalesTodayYTDMTD",
        branchCode: @Html.Raw(Json.Encode(ViewBag.BranchCode)),
    companyCode: @Html.Raw(Json.Encode(ViewBag.CompanyCode)),
    UserId: @Html.Raw(Json.Encode(ViewBag.UserId)),
    //fromDate: "17-Jul-2024",
    //toDate: "16-Jul-2025",
    fromDate: @Html.Raw(Json.Encode(ViewBag.StartFromDate)),
    toDate: @Html.Raw(Json.Encode(ViewBag.EndToDate)),
    itemStringFilter: [],
        customerCode: "",
            dealerCode: "",
                months: ["This Month", "This Year", "Last Month"],
                    cat: "",
                        brand: "",
                            grp: "",
                                grossNet: "",
                                    grossNetStr: "",
                                        brand: "",
                                            cusGrp: "",
                                                cusGrpStr: "",
                                                    unitType: "",
                                                        with_eng_date: ""
};
try {
    const response = await fetch(apiUrl, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(params)
    });

    if (!response.ok) {
        throw new Error('Network response was not ok');
    }
    const data = await response.json();
    updateSalesDataSearch(data, reportType);
} catch (error) {
    console.error('Error fetching sales data:', error);
}
    }
function updateSalesDataSearch(data) {

    //Sales Data
    if (data.hasOwnProperty('sales_Yesterday')) {
        document.getElementById('h1dYeserday').innerText = 'Yesterday';
        document.getElementById('p1dYeserday').innerText = data['sales_Yesterday'].TotalAmt.toFixed(2);
    }
    else {
        document.getElementById('h1dYeserday').innerText = 'Yesterday';
        document.getElementById('p1dYeserday').innerText = '0';
    }

    if (data.hasOwnProperty('sales_Yesterday')) {
        document.getElementById('h1dToday').innerText = 'Today';
        document.getElementById('p1dToday').innerText = data['sales_FRI-09/27'].TotalAmt.toFixed(2);
    }
    else {
        document.getElementById('h1dToday').innerText = 'Today';
        document.getElementById('p1dToday').innerText = '0';
    }

    if (data.hasOwnProperty('sales_This_Month')) {
        document.getElementById('h1MTDToday').innerText = 'This Month';
        document.getElementById('p1MTDToday').innerText = data['sales_This_Month'].TotalAmt.toFixed(2);
    }
    else {
        document.getElementById('h1MTDToday').innerText = 'This Month';
        document.getElementById('p1MTDToday').innerText = '0';
    }
    if (data.hasOwnProperty('sales_This Year')) {
        document.getElementById('h3YTD').innerText = 'This Year';
        document.getElementById('P3YTD').innerText = Math.round(data['sales_This Year'].TotalAmt.toFixed(2));
    }
    else {
        document.getElementById('h3YTD').innerText = 'This Year';
        document.getElementById('P3YTD').innerText = '0';
    }

    //Week Data
    if (data.hasOwnProperty('sales_This_Month')) {
        document.getElementById('Weekh2dYeserday').innerText = '09/24';
        document.getElementById('Weekp2dYeserday').innerText = data['sales_This_Month'].TotalAmt.toFixed(2);
    }
    else {
        document.getElementById('Weekh2dYeserday').innerText = '09/24';
        document.getElementById('Weekp2dYeserday').innerText = '0';
    }
    if (data.hasOwnProperty('sales_This_Month')) {
        document.getElementById('Weekh2dToday').innerText = '10/25';
        document.getElementById('Weekp2dToday').innerText = data['sales_This_Month'].TotalAmt.toFixed(2);
    }
    else {
        document.getElementById('Weekh2dToday').innerText = '10/25';
        document.getElementById('Weekp2dToday').innerText = "0";
    }
    if (data.hasOwnProperty('sales_This_Month')) {
        document.getElementById('Weekh2MTDToday').innerText = '09/26';
        document.getElementById('Weekp2MTDToday').innerText = data['sales_This Year'].TotalAmt.toFixed(2);
    }
    else {
        document.getElementById('Weekh2MTDToday').innerText = '09/26';
        document.getElementById('Weekp2MTDToday').innerText = '0';
    }
    if (data.hasOwnProperty('sales_This_Month')) {
        document.getElementById('Weekh2YTD').innerText = '09/27';
        document.getElementById('WeekP2YTD').innerText = data['sales_This Year'].TotalAmt.toFixed(2);
    }
    else {
        document.getElementById('Weekh2YTD').innerText = '09/27';
        document.getElementById('WeekP2YTD').innerText = '0';
    }

    if (data.hasOwnProperty('sales_This_Month')) {
        document.getElementById('Weekh3dToday').innerText = '09/28';
        document.getElementById('Weekp3dToday').innerText = data['sales_This Year'].TotalAmt.toFixed(2);
    }
    else {
        document.getElementById('Weekh3dToday').innerText = '09/28';
        document.getElementById('Weekp3dToday').innerText = '0';
    }
    if (data.hasOwnProperty('sales_This_Month')) {
        document.getElementById('Weekh3MTDToday').innerText = '09/29';
        document.getElementById('Weekp3MTDToday').innerText = data['sales_This Year'].TotalAmt.toFixed(2);
    }
    else {
        document.getElementById('Weekh3MTDToday').innerText = '09/29';
        document.getElementById('Weekp3MTDToday').innerText = '0';
    }
    if (data.hasOwnProperty('sales_This_Month')) {
        document.getElementById('Weekh3YTD').innerText = '09/30';
        document.getElementById('WeekP3YTD').innerText = data['sales_This Year'].TotalAmt.toFixed(2);
    }
    else {
        document.getElementById('Weekh3YTD').innerText = '09/30';
        document.getElementById('WeekP3YTD').innerText = '0';
    }
}

//ctxChart = null;
monthlySalesChartInstance;
reportType;
reportTypePeriodic = "This Month";

$(document).ready(function () {
    $(document).on("click", ".dropdown-item-two", function (e) {
        e.preventDefault();
        const reportType = $(this).data("report-type");
        alert(reportType);
    });
});

const Weekly_BASE_URL_Sales = @Html.Raw(Json.Encode(ViewBag.BaseUrlForDashboard));
async function UpdateSalesTodayYTDMTD(reportType) {
    const apiUrl = `${Weekly_BASE_URL_Sales}/api/Indicatorpanel/GetSalesTodayYTDMTD`;
    const params = {
        repType: "GetSalesTodayYTDMTD",
        branchCode: @Html.Raw(Json.Encode(ViewBag.BranchCode)),
    companyCode: @Html.Raw(Json.Encode(ViewBag.CompanyCode)),
    UserId: @Html.Raw(Json.Encode(ViewBag.UserId)),
    //fromDate: "17-Jul-2024",
    //toDate: "16-Jul-2025",
    fromDate: @Html.Raw(Json.Encode(ViewBag.StartFromDate)),
    toDate: @Html.Raw(Json.Encode(ViewBag.EndToDate)),
    itemStringFilter: [],
        customerCode: "",
            dealerCode: "",
                months: ["This Month", "This Year", "Last Month"],
                    cat: "",
                        brand: "",
                            grp: "",
                                grossNet: "",
                                    grossNetStr: "",
                                        brand: "",
                                            cusGrp: "",
                                                cusGrpStr: "",
                                                    unitType: "",
                                                        with_eng_date: ""
};
try {
    const response = await fetch(apiUrl, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(params)
    });

    if (!response.ok) {
        throw new Error('Network response was not ok');
    }
    const data = await response.json();
    updateSalesDataSalesTodayYTDMTDSearch(data, reportType);
} catch (error) {
    console.error('Error fetching sales data:', error);
}
    }
function updateSalesDataSalesTodayYTDMTDSearch(data) {

    //Sales Data
    if (data.hasOwnProperty('sales_Yesterday')) {
        document.getElementById('h1dYeserday').innerText = 'Yesterday';
        document.getElementById('p1dYeserday').innerText = data['sales_Yesterday'].TotalAmt.toFixed(2);
    }
    else {
        document.getElementById('h1dYeserday').innerText = 'Yesterday';
        document.getElementById('p1dYeserday').innerText = '0';
    }

    if (data.hasOwnProperty('sales_Yesterday')) {
        document.getElementById('h1dToday').innerText = 'Today';
        document.getElementById('p1dToday').innerText = data['sales_FRI-09/27'].TotalAmt.toFixed(2);
    }
    else {
        document.getElementById('h1dToday').innerText = 'Today';
        document.getElementById('p1dToday').innerText = '0';
    }

    if (data.hasOwnProperty('sales_This_Month')) {
        document.getElementById('h1MTDToday').innerText = 'This Month';
        document.getElementById('p1MTDToday').innerText = data['sales_This_Month'].TotalAmt.toFixed(2);
    }
    else {
        document.getElementById('h1MTDToday').innerText = 'This Month';
        document.getElementById('p1MTDToday').innerText = '0';
    }
    if (data.hasOwnProperty('sales_This Year')) {
        document.getElementById('h3YTD').innerText = 'This Year';
        document.getElementById('P3YTD').innerText = Math.round(data['sales_This Year'].TotalAmt.toFixed(2));
    }
    else {
        document.getElementById('h3YTD').innerText = 'This Year';
        document.getElementById('P3YTD').innerText = '0';
    }

    //Week Data
    if (data.hasOwnProperty('sales_This_Month')) {
        document.getElementById('Weekh2dYeserday').innerText = '09/24';
        document.getElementById('Weekp2dYeserday').innerText = data['sales_This_Month'].TotalAmt.toFixed(2);
    }
    else {
        document.getElementById('Weekh2dYeserday').innerText = '09/24';
        document.getElementById('Weekp2dYeserday').innerText = '0';
    }
    if (data.hasOwnProperty('sales_This_Month')) {
        document.getElementById('Weekh2dToday').innerText = '10/25';
        document.getElementById('Weekp2dToday').innerText = data['sales_This_Month'].TotalAmt.toFixed(2);
    }
    else {
        document.getElementById('Weekh2dToday').innerText = '10/25';
        document.getElementById('Weekp2dToday').innerText = "0";
    }
    if (data.hasOwnProperty('sales_This_Month')) {
        document.getElementById('Weekh2MTDToday').innerText = '09/26';
        document.getElementById('Weekp2MTDToday').innerText = data['sales_This Year'].TotalAmt.toFixed(2);
    }
    else {
        document.getElementById('Weekh2MTDToday').innerText = '09/26';
        document.getElementById('Weekp2MTDToday').innerText = '0';
    }
    if (data.hasOwnProperty('sales_This_Month')) {
        document.getElementById('Weekh2YTD').innerText = '09/27';
        document.getElementById('WeekP2YTD').innerText = data['sales_This Year'].TotalAmt.toFixed(2);
    }
    else {
        document.getElementById('Weekh2YTD').innerText = '09/27';
        document.getElementById('WeekP2YTD').innerText = '0';
    }

    if (data.hasOwnProperty('sales_This_Month')) {
        document.getElementById('Weekh3dToday').innerText = '09/28';
        document.getElementById('Weekp3dToday').innerText = data['sales_This Year'].TotalAmt.toFixed(2);
    }
    else {
        document.getElementById('Weekh3dToday').innerText = '09/28';
        document.getElementById('Weekp3dToday').innerText = '0';
    }
    if (data.hasOwnProperty('sales_This_Month')) {
        document.getElementById('Weekh3MTDToday').innerText = '09/29';
        document.getElementById('Weekp3MTDToday').innerText = data['sales_This Year'].TotalAmt.toFixed(2);
    }
    else {
        document.getElementById('Weekh3MTDToday').innerText = '09/29';
        document.getElementById('Weekp3MTDToday').innerText = '0';
    }
    if (data.hasOwnProperty('sales_This_Month')) {
        document.getElementById('Weekh3YTD').innerText = '09/30';
        document.getElementById('WeekP3YTD').innerText = data['sales_This Year'].TotalAmt.toFixed(2);
    }
    else {
        document.getElementById('Weekh3YTD').innerText = '09/30';
        document.getElementById('WeekP3YTD').innerText = '0';
    }
}

ctxChart = null;
monthlySalesChartInstance;
reportType;
reportTypePeriodic = "This Month";

$(document).ready(function () {
    $(document).on("click", ".dropdown-item-three", function (e) {
        e.preventDefault();
        const reportType = $(this).data("report-type");
        alert(reportType);
    });
});
const Overview_BASE_URL = @Html.Raw(Json.Encode(ViewBag.BaseUrlForDashboard));
function getOverViewData(selectedMonth) {
    const url = `${Overview_BASE_URL}/api/Indicatorpanel/Overview`;
    const requestData = {
        repType: "Overview",
        branchCode: @Html.Raw(Json.Encode(ViewBag.BranchCode)),
    companyCode: @Html.Raw(Json.Encode(ViewBag.CompanyCode)),
    UserId: @Html.Raw(Json.Encode(ViewBag.UserId)),
    @* fromDate: @Html.Raw(Json.Encode(ViewBag.StartFromDate)),
    toDate: @Html.Raw(Json.Encode(ViewBag.EndToDate)),*@
    //fromDate: "17-Jul-2023",
    //toDate: "16-Jul-2024",
    fromDate: @Html.Raw(Json.Encode(ViewBag.StartFromDate)),
    toDate: @Html.Raw(Json.Encode(ViewBag.EndToDate)),
    itemStringFilter: [],
        customerCode: "",
            dealerCode: "",
                months: [selectedMonth],
                    cat: "",
                        brand: "",
                            grp: "",
                                grossNet: "",
                                    grossNetStr: "",
                                        brand: "",
                                            cusGrp: "",
                                                cusGrpStr: "",
                                                    unitType: "",
                                                        with_eng_date: ""
};
fetch(url, {
    method: "POST",
    headers: {
        "Content-Type": "application/json"
    },
    body: JSON.stringify(requestData)
})
    .then(response => response.json())
    .then(data => {
        data.forEach(item => {
            switch (item.topic) {
                //    case "Sales":
                //        document.getElementById("salesAmount").textContent = Math.round(item.amt);

                //        break;

                //    case "Collection":
                //        document.getElementById("collectionAmount").textContent = 6670000;
                //        break;

                //    case "Purchase":
                //        document.getElementById("purchaseAmount").textContent = 3100000;
                //        break;

                //    case "Production":
                //        document.getElementById("productionAmount").textContent = 7660000;
                //        break;

                //    case "Cash Balance":
                //        document.getElementById("cashBalanceAmount").textContent = 975500;
                //        break;

                //    case "Bank Balance":
                //        document.getElementById("bankBalanceAmount").textContent = 8722000;
                //        break;

                //    case "Receivable":
                //        document.getElementById("receivableAmount").textContent = 7445007;
                //        break;

                //    case "Payable":
                //        document.getElementById("payableAmount").textContent = 6732800;
                //        break;

                //    case "Loan":
                //        document.getElementById("loanAmount").textContent = 349900;
                //        break;

                //    case "VAT":
                //        document.getElementById("vatAmount").textContent = 673900;
                //        break;

                //    case "Expenses":
                //        document.getElementById("expensesAmount").textContent = 860056;
                //        break;

                //    case "ClosingStock":
                //        document.getElementById("closingStockAmount").textContent = 765590;
                //        break;
            }
        });
    })
    .catch(error => {
        console.error("Error fetching sales data:", error);
    });
        }

async function loadPluginPage(page) {
    const pageMapping = {
        Sales: '@Url.Action("SalesHome/DashBoard", "Sales")',
        @* Sales: '@Url.Action("SalesHome/_dashboardPartialView", "Sales")',*@
    AnalyticsGraph: '@Url.Action("SalesHome/_dashboardChartView", "Sales")',
        //performance: '@Url.Action("SalesHome/_RenderDashboardMenu", "Sales")',
        performance: '@Url.Action("SalesHome/RenderDashboardMenu", "Sales")',
            Distribution: '@Url.Action("dashboardlayout", "Home")',
                DocumentTemplate: '@Url.Action("Dashboard", "DocumentTemplate")',
                    LOC: '@Url.Action("Index", "Loc")',
                        Planning: '@Url.Action("PlanningDashboard", "Home")',
                            ProjectManagement: '@Url.Action("ProjectDashboard", "Home")',
                                QuotationManagement: '@Url.Action("Dashboard", "QuotationManagement")'
};

const pageUrl = pageMapping[page];
if (!pageUrl) {
    console.error("Invalid page requested:", page);
    return;
}

if ($('.contentPlaceHolder').length === 0) {
    console.error("Content container not found.");
    return;
}
try {
    $.ajax({
        url: pageUrl,
        type: 'GET',
        data: { id: 123 },
        success: function (response) {
            $('.contentPlaceHolder').html(response);
        },
        error: function (xhr, status, error) {
            console.error("Error loading content:", error);
            $('.contentPlaceHolder').html(`
                    <div class="alert alert-danger" role="alert">
                        Unable to load the requested content. Please try again later.
                    </div>
                `);
        }
    });
} catch (error) {
    console.error("Unexpected error:", error);
}


       //$.ajax({
            //    url: performance,
            //    type: 'GET',
            //    data: { id: 123 },
            //    success: function (response) {
            //        $('.contentPlaceHolder').html(response);
            //    },
            //    error: function () {
            //        alert('Error loading dashboard content');
            //    }
            //});

            //const pageUrl = pageMapping[page];
            //if (!pageUrl) {
            //    console.error("Invalid page requested:", page);
            //    return;
            //}

            //const contentContainer = document.querySelector('.contentPlaceHolder');
            //if (!contentContainer) {
            //    console.error("Content container not found.");
            //    return;
            //}


            //contentContainer.innerHTML = `
            //    <div class="spinner-border text-primary" role="status">
            //        <span class="visually-hidden">Loading...</span>
            //    </div>
            //`;
            //try {
            //    const response = await fetch(pageUrl, {
            //        method: 'GET',
            //        headers: {
            //            'Content-Type': 'text/html',
            //        },
            //    });
            //    if (!response.ok) {
            //        throw new Error(`Failed to fetch page. HTTP Status: ${response.status}`);
            //    }
            //    const html = await response.text();
            //    contentContainer.innerHTML = html;
            //} catch (error) {
            //    console.error("Error fetching page:", error);
            //    contentContainer.innerHTML = `
            //        <div class="alert alert-danger" role="alert">
            //            Unable to load the requested content. Please try again later.
            //        </div>
            //    `;
            // }
        }

document.querySelectorAll('.accordion-button').forEach(button => {
    button.addEventListener('click', () => {
        const target = button.getAttribute('data-bs-target');
        const collapseElement = document.querySelector(target);

        if (collapseElement.classList.contains('show')) {
            collapseElement.classList.remove('show');
        } else {
            document.querySelectorAll('.accordion-collapse').forEach(el => el.classList.remove('show'));
            collapseElement.classList.add('show');
        }
    });
});

/* let ctxChart = null;*/
//let monthlySalesChartInstance;
//let reportType;
//let reportTypePeriodic = "This Month";

document.querySelectorAll('.dropdown-item').forEach(item => {
    item.addEventListener('click', function (e) {
        reportType = e.target.getAttribute('data-report-type');
        updateChart(reportType);
    });
});
function updateChart(reportType) {
    let chartType;
    switch (reportType) {
        case 'LineChart':
            chartType = 'line';
            break;
        case 'PieChart':
            chartType = 'pie';
            break;
        case 'ColumnChart':
            chartType = 'column';
            break;
        case 'TableView':
            chartType = 'table';
            break;
        case 'PivotTable':
            chartType = 'pivot';
            break;
        default:
            chartType = 'line';
    }
    fetchLineChartRender(chartType, reportTypePeriodic);
}

async function fetchLineChartRender(chartType, reportTypePeriodic) {
    const ChartBASE_URL = @Html.Raw(Json.Encode(ViewBag.BaseUrlForDashboard));
    try {
        const requestData = {
            repType: "graph_wise_sales",
            branchCode: @Html.Raw(Json.Encode(ViewBag.BranchCode)),
        companyCode: @Html.Raw(Json.Encode(ViewBag.CompanyCode)),
        UserId: @Html.Raw(Json.Encode(ViewBag.UserId)),
        //fromDate: "17-Jul-2023",
        //toDate: "16-Jul-2024",
        itemStringFilter: [],
            customerCode: "",
                dealerCode: "",
                    months: ["This Month", "This Year", "Last Month"],
                        cat: "",
                            brand: "",
                                grp: "",
                                    grossNet: "",
                                        grossNetStr: "",
                                            cusGrp: "",
                                                cusGrpStr: "",
                                                    unitType: "",
                                                        with_eng_date: ""
    };

    const response = await fetch(`${ChartBASE_URL}/api/Indicatorpanel/GetGraphWiseSalesReport`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(requestData)
    });

    const data = await response.json();
    const labelSalesChart = data.com_wise_sales.details.map(branch => branch.name);
    const amountSalesChart = data.com_wise_sales.details.map(branch => branch.amt);
    const quantitieSalesChart = data.com_wise_sales.details.map(branch => branch.qty);

    switch (chartType) {
        case 'line':
            renderLineChart(labelSalesChart, amountSalesChart, quantitieSalesChart);
            break;
        case 'bar':
            renderBarChart(labelSalesChart, amountSalesChart);
            break;
        case 'pie':
            renderPieChart(labelSalesChart, amountSalesChart);
            break;
        case 'table':
            renderTableView(data.com_wise_sales.details);
            break;
        case 'column':
            renderColumnChart(labelSalesChart, amountSalesChart, quantitieSalesChart);
            break;
        case 'pivot':
            renderPivotTable(data.com_wise_sales.details);
            break;
        default:
            console.error('Unknown chart type');
    }
} catch (error) {
    console.error("Error fetching sales data:", error);
}
            }

function renderLineChart(labelSalesChart, amountSalesChart, quantitieSalesChart) {
    if (ctxChart) {
        ctxChart.destroy();
    }
    const ctxmonthlySalesChart = document.getElementById('monthlySalesLineChart').getContext('2d');
    ctxChart = new Chart(ctxmonthlySalesChart, {
        type: 'line',
        data: {
            labels: labelSalesChart,
            datasets: [
                {
                    label: 'Sales Amount',
                    data: amountSalesChart,
                    borderColor: 'rgba(58, 123, 213, 1)',
                    backgroundColor: 'rgba(58, 123, 213, 0.2)',
                    fill: true,
                    borderWidth: 2,
                    tension: 0.4
                },
                {
                    label: 'Quantity Sold',
                    data: quantitieSalesChart,
                    borderColor: 'rgba(255, 159, 64, 1)',
                    backgroundColor: 'rgba(255, 159, 64, 0.2)',
                    fill: true,
                    borderWidth: 2,
                    tension: 0.4
                }
            ]
        },
        options: {
            scales: {
                x: {
                    grid: { color: 'rgba(255, 255, 255, 0.1)' },
                    ticks: { color: 'white' }
                },
                y: {
                    grid: { color: 'rgba(255, 255, 255, 0.1)' },
                    ticks: { color: 'white' }
                }
            },
            plugins: {
                legend: { labels: { color: 'white' } },
                tooltip: {
                    backgroundColor: 'rgba(0, 0, 0, 0.7)',
                    titleColor: 'white',
                    bodyColor: 'white',
                    borderColor: 'rgba(75, 192, 192, 1)',
                    borderWidth: 1
                },
                title: {
                    display: true,
                    text: 'Branch Wise Sales',
                    color: '#714b67',
                    font: { size: 12, weight: 'bold' }
                }
            },
            responsive: true
        }
    });
    toggleChartVisibility('line');
}

function renderBarChart(labels, amounts) {
    if (ctxChart) {
        ctxChart.destroy();
    }
    ctxMonthlySalesBarChart = document.getElementById('monthlySalesBarChart').getContext('2d');
    ctxChart = new Chart(ctxMonthlySalesBarChart, {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [
                {
                    label: 'Amount',
                    data: amounts,
                    backgroundColor: 'rgba(54, 162, 235, 0.6)',
                    borderColor: 'rgba(54, 162, 235, 1)',
                    borderWidth: 1,
                },
            ],
        },
        options: {
            indexAxis: 'y',
        },
    });
}

//    function renderPieChart(labelSalesChart, amountSalesChart) {
//        if (ctxChart) {
//            ctxChart.destroy();
//        }
//        const salesChart = document.getElementById('monthlySalesPieChart').getContext('2d');
//        ctxChart = new Chart(salesChart, {
//        type: 'pie',
//        data: {
//            labels: labelSalesChart,
//            datasets: [{
//                label: 'Sales Amount',
//                data: amountSalesChart,
//                backgroundColor: ['rgba(255, 99, 132, 0.2)', 'rgba(54, 162, 235, 0.2)', 'rgba(255, 206, 86, 0.2)', 'rgba(75, 192, 192, 0.2)'],
//                borderColor: ['rgba(255, 99, 132, 1)', 'rgba(54, 162, 235, 1)', 'rgba(255, 206, 86, 1)', 'rgba(75, 192, 192, 1)'],
//                borderWidth: 1
//            }]
//        },
//        options: {
//            plugins: {
//                legend: { display: true, position: 'top', labels: { color: 'white' } },
//                title: {
//                    display: true,
//                    text: `Pie Chart - ${reportTypePeriodic || ''}`,
//                    color: 'white',
//                    font: { size: 14, weight: 'bold' }
//                },
//                tooltip: {
//                    backgroundColor: 'rgba(0, 0, 0, 0.7)',
//                    titleColor: 'white',
//                    bodyColor: 'white'
//                }
//            },
//            responsive: true
//        }
//    });
//    toggleChartVisibility('pie');
//}

//function renderPieChart(labelSalesChart, amountSalesChart) {
//    if (ctxChart) {
//        ctxChart.destroy();
//    }
//    const salesChartCanvas = document.getElementById('monthlySalesPieChart');
//    salesChartCanvas.width = 400;
//    salesChartCanvas.height = 300;
//    const ctx = salesChartCanvas.getContext('2d');
//    ctxChart = new Chart(ctx, {
//        type: 'pie',
//        data: {
//            labels: labelSalesChart,
//            datasets: [{
//                label: 'Sales Amount',
//                data: amountSalesChart,
//                backgroundColor: [
//                    'rgba(255, 99, 132, 0.2)',
//                    'rgba(54, 162, 235, 0.2)',
//                    'rgba(255, 206, 86, 0.2)',
//                    'rgba(75, 192, 192, 0.2)'
//                ],
//                borderColor: [
//                    'rgba(255, 99, 132, 1)',
//                    'rgba(54, 162, 235, 1)',
//                    'rgba(255, 206, 86, 1)',
//                    'rgba(75, 192, 192, 1)'
//                ],
//                borderWidth: 1
//            }]
//        },
//        options: {
//            plugins: {
//                legend: {
//                    display: true,
//                    position: 'top',
//                    labels: {
//                        color: 'white'
//                    }
//                },
//                title: {
//                    display: true,
//                    text: `Pie Chart - ${reportTypePeriodic || ''}`,
//                    color: 'white',
//                    font: {
//                        size: 14,
//                        weight: 'bold'
//                    }
//                },
//                tooltip: {
//                    backgroundColor: 'rgba(0, 0, 0, 0.7)',
//                    titleColor: 'white',
//                    bodyColor: 'white'
//                }
//            },
//            responsive: true
//        }
//    });

//    toggleChartVisibility('pie');
//}


/*    Chart.register(ChartDataLabels);*/
function renderPieChart(labelSalesChart, amountSalesChart) {
    if (ctxChart) {
        ctxChart.destroy();
    }

    const salesChartCanvas = document.getElementById('monthlySalesPieChart');
    salesChartCanvas.width = 400;
    salesChartCanvas.height = 300;

    const ctx = salesChartCanvas.getContext('2d');

    const colors = [
        'rgba(63, 81, 181, 0.7)', // Blue
        'rgba(33, 150, 243, 0.7)', // Light Blue
        'rgba(76, 175, 80, 0.7)',  // Green
        'rgba(255, 193, 7, 0.7)',  // Yellow
        'rgba(156, 39, 176, 0.7)', // Purple
        'rgba(244, 67, 54, 0.7)',  // Red
        'rgba(255, 87, 34, 0.7)',  // Orange
        'rgba(121, 85, 72, 0.7)'   // Brown
    ];

    ctxChart = new Chart(ctx, {
        type: 'pie',
        data: {
            labels: labelSalesChart,
            datasets: [{
                label: 'Sales Amount',
                data: amountSalesChart,
                backgroundColor: colors,
                borderColor: colors.map(color => color.replace('0.7', '1')),
                borderWidth: 1
            }]
        },
        options: {
            plugins: {
                legend: {
                    display: true,
                    position: 'top',
                    labels: {
                        color: '#333',
                        font: {
                            size: 12
                        }
                    }
                },
                title: {
                    display: true,
                    text: `Pie Chart - ${reportTypePeriodic || ''}`,
                    color: '#333',
                    font: {
                        size: 14,
                        weight: 'bold'
                    }
                },
                tooltip: {
                    backgroundColor: 'rgba(0, 0, 0, 0.7)',
                    titleColor: 'white',
                    bodyColor: 'white'
                },
                datalabels: {
                    display: true,
                    color: 'white',
                    formatter: (value, context) => {
                        const total = context.chart._metasets[0].total;
                        const percentage = ((value / total) * 100).toFixed(2) + '%';
                        return `${value} (${percentage})`;
                    },
                    font: {
                        weight: 'bold',
                        size: 12
                    },
                    anchor: 'center',
                    align: 'center'
                }
            },
            responsive: true
        }
    });

    toggleChartVisibility('pie');
}
//    function renderColumnChart(labels, amounts, quantities) {
//        if (ctxChart) {
//            ctxChart.destroy();
//        }
//        const currentChart= document.getElementById('monthlySalesColumnChart').getContext('2d');
//        ctxChart = new Chart(currentChart, {
//        type: 'bar',
//        data: {
//            labels: labels,
//            datasets: [
//                {
//                    label: 'Amount',
//                    data: amounts,
//                    backgroundColor: 'rgba(75, 192, 192, 0.6)',
//                    borderColor: 'rgba(75, 192, 192, 1)',
//                    borderWidth: 1,
//                },
//                {
//                    label: 'Quantity',
//                    data: quantities,
//                    backgroundColor: 'rgba(255, 99, 132, 0.6)',
//                    borderColor: 'rgba(255, 99, 132, 1)',
//                    borderWidth: 1,
//                },
//            ],
//        },
//    });
//}


/* Chart.register(ChartDataLabels);*/
function renderColumnChart(labels, amounts, quantities) {
    if (ctxChart) {
        ctxChart.destroy();
    }

    const currentChart = document.getElementById('monthlySalesColumnChart').getContext('2d');
    ctxChart = new Chart(currentChart, {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [
                {
                    label: 'Amount',
                    data: amounts,
                    backgroundColor: 'rgba(33, 150, 243, 0.6)',
                    borderColor: 'rgba(33, 150, 243, 1)',
                    borderWidth: 1,
                },
                {
                    label: 'Quantity',
                    data: quantities,
                    backgroundColor: 'rgba(76, 175, 80, 0.6)',
                    borderColor: 'rgba(76, 175, 80, 1)',
                    borderWidth: 1,
                }
            ]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    display: true,
                    position: 'top',
                    labels: {
                        color: '#333',
                        font: {
                            size: 12
                        }
                    }
                },
                title: {
                    display: true,
                    text: 'Sales Data - Amount & Quantity',
                    color: '#333',
                    font: {
                        size: 14,
                        weight: 'bold'
                    }
                },
                tooltip: {
                    backgroundColor: 'rgba(0, 0, 0, 0.7)',
                    titleColor: 'white',
                    bodyColor: 'white'
                },
                datalabels: {
                    display: true,
                    color: 'white',
                    formatter: (value, context) => {
                        return `${value}`;
                    },
                    font: {
                        weight: 'bold',
                        size: 12
                    },
                    anchor: 'center',
                    align: 'center'
                }
            },
            scales: {
                x: {
                    ticks: {
                        color: '#333',
                    }
                },
                y: {
                    ticks: {
                        color: '#333',
                        beginAtZero: true,
                    }
                }
            },
            barPercentage: 0.8,
            categoryPercentage: 0.7,
        }
    });
}

function renderTableView(details) {
    const tbody = document.getElementById('salesTableBody');
    if (!tbody) {
        console.error('Table body element with id "salesTableBody" not found.');
        return;
    }
    tbody.innerHTML = '';
    if (details && details.length > 0) {
        details.forEach((branch, index) => {
            const row = document.createElement('tr');
            row.className = index % 2 === 0 ? 'even-row' : 'odd-row';
            const formattedAmount = new Intl.NumberFormat('en-NP', {
                style: 'currency',
                currency: 'NPR',
                currencyDisplay: 'code',
            }).format(branch.amt);
            const customFormattedAmount = formattedAmount.replace('NPR', 'Rs.');

            row.innerHTML = `
                <td style="width:500px">${branch.name}</td>
                <td style="width:230px" class="text-right">${customFormattedAmount}</td>
                <td style="width:230px" class="text-right">${branch.qty}</td>
            `;
            tbody.appendChild(row);
        });
    } else {

        const row = document.createElement('tr');
        row.innerHTML = `<td colspan="3" class="no-data">No data available</td>`;
        tbody.appendChild(row);
    }
    toggleChartVisibility('table');
}

function renderPivotTable(details) {
    const tbody = document.getElementById('salesPivotTableBody');
    if (!tbody) {
        console.error('Table body element with id "salesPivotTableBody" not found.');
        return;
    }
    tbody.innerHTML = '';

    if (details && details.length > 0) {
        const pivotData = details.reduce((acc, branch) => {
            if (!acc[branch.name]) {
                acc[branch.name] = { name: branch.name, totalAmt: 0, totalQty: 0 };
            }
            acc[branch.name].totalAmt += branch.amt;
            acc[branch.name].totalQty += branch.qty;
            return acc;
        }, {});

        const groupedDetails = Object.values(pivotData);

        groupedDetails.forEach((group, index) => {
            const row = document.createElement('tr');
            row.className = index % 2 === 0 ? 'even-row' : 'odd-row';

            const formattedAmount = new Intl.NumberFormat('en-NP', {
                style: 'currency',
                currency: 'NPR',
                currencyDisplay: 'code',
            }).format(branch.amt);
            const customFormattedAmount = formattedAmount.replace('NPR', 'Rs.');

            row.innerHTML = `
        <td style="width:500px">${group.name}</td>
        <td style="width:240px" class="text-right">${customFormattedAmount}</td>
        <td style="width:230px" class="text-right">${group.totalQty}</td>
        `;
            tbody.appendChild(row);
        });
    } else {
        const row = document.createElement('tr');
        row.innerHTML = `<td colspan="3" class="no-data">No data available</td>`;
        tbody.appendChild(row);
    }
    const table = document.getElementById('salesPivotTable');
    if (table) {
        table.style.display = 'table';
    }
}

function clearCanvas() {
    if (ctxmonthlySalesChart) {
        ctxmonthlySalesChart.clearRect(0, 0, ctxmonthlySalesChart.canvas.width, ctxmonthlySalesChart.canvas.height);
    }
}

function clearCanvas(chartId) {
    const canvas = document.getElementById(chartId);
    if (canvas && monthlySalesChartInstance) {
        monthlySalesChartInstance.destroy();
        monthlySalesChartInstance = null;
    }
}

function toggleChartVisibility(chartType) {
    const charts = ['monthlySalesChart', 'monthlySalesLineChart', 'monthlySalesPieChart', 'monthlySalesBarChart', 'monthlySalesColumnChart', 'salesTable', 'salesPivotTable'];
    charts.forEach(chart => {
        document.getElementById(chart).style.display = 'none';
    });

    switch (chartType) {
        case 'line':
            document.getElementById('monthlySalesLineChart').style.display = 'block';
            break;
        case 'pie':
            document.getElementById('monthlySalesPieChart').style.display = 'block';
            break;
        case 'bar':
            document.getElementById('monthlySalesBarChart').style.display = 'block';
            break;
        case 'table':
            document.getElementById('salesTable').style.display = 'block';
            break;
        case 'column':
            document.getElementById('monthlySalesColumnChart').style.display = 'block';
            break;
        case 'pivot':
            document.getElementById('salesPivotTable').style.display = 'block';
            break;
        default:
            console.error('Unknown chart type for visibility toggling');
    }
}
