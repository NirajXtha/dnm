function startGlobalDashboardTour() {
    if (typeof window.driver === 'undefined' || typeof window.driver.js === 'undefined') {
        console.warn('Driver.js is not loaded. Tour cannot be started.');
        return;
    }

    if (localStorage.getItem('globalDashboardTourCompleted') === 'true') {
        return;
    }

    setTimeout(function () {
        initializeTour();
    }, 1000);
}

function initializeTour() {
    var driver = window.driver.js.driver;

    if (!driver) {
        console.warn('Driver.js driver function not found.');
        return;
    }

    function findElementByText(selector, text) {
        var elements = document.querySelectorAll(selector);
        for (var i = 0; i < elements.length; i++) {
            if (elements[i].textContent.trim().toUpperCase() === text.toUpperCase()) {
                return elements[i];
            }
        }
        return null;
    }

    var steps = [];

    if (document.querySelector('.page-logo')) {
        steps.push({
            element: '.page-logo',
            popover: {
                title: 'Welcome to Synergy ERP!',
                description: 'This is company logo. Click here anytime to return to the Global Dashboard.',
                side: 'bottom',
                align: 'start'
            }
        });
    }

    if (document.querySelector('.user-header')) {
        steps.push({
            element: '.user-header',
            popover: {
                title: 'Company & Branch',
                description: 'This displays current company name and the branch you are logged into.',
                side: 'bottom',
                align: 'start'
            }
        });
    }

    var aiElement = findElementByText('a', 'AI Assistant') ||
        findElementByText('span', 'AI Assistant') ||
        findElementByText('li', 'AI Assistant') ||
        findElementByText('.nav-link', 'AI Assistant');

    if (aiElement) {
        steps.push({
            element: aiElement,
            popover: {
                title: 'AI Assistant',
                description: 'Click here to access the AI Assistant for intelligent help and insights.',
                side: 'bottom',
                align: 'center'
            }
        });
    }

    if (document.querySelector('#header_inbox_bar')) {
        steps.push({
            element: '#header_inbox_bar',
            popover: {
                title: 'Full Screen Mode',
                description: 'Click here to toggle full screen mode for a better view of your dashboard.',
                side: 'bottom',
                align: 'center'
            }
        });
    }

    if (document.querySelector('.dropdown-user')) {
        steps.push({
            element: '.dropdown-user',
            popover: {
                title: 'User Menu',
                description: 'Click here to access your profile, change password, or log out from the system.',
                side: 'left',
                align: 'start'
            }
        });
    }

    if (document.querySelector('.dropdown-quick-sidebar-toggler')) {
        steps.push({
            element: '.dropdown-quick-sidebar-toggler',
            popover: {
                title: 'Right Panel',
                description: 'Access the quick sidebar for additional tools and notifications.',
                side: 'left',
                align: 'start'
            }
        });
    }

    var biElement = findElementByText('.caption-subject.bold.uppercase.custom-modal', 'BUSINESS INTELLIGENCE TOOL') ||
        findElementByText('.caption-subject', 'BUSINESS INTELLIGENCE TOOL');

    if (biElement) {
        steps.push({
            element: biElement,
            popover: {
                title: 'Business Intelligence',
                description: 'Here you can access sales reports, organizers, and perform comparative analysis using charts.',
                side: 'top',
                align: 'center'
            }
        });
    }
    var distElement = findElementByText('.caption-subject.bold.uppercase.custom-modal', 'DISTRIBUTION') ||
        findElementByText('.caption-subject', 'DISTRIBUTION');
    if (distElement) {
        steps.push({
            element: distElement,
            popover: {
                title: 'Distribution',
                description: 'Here is distribution module.',
                side: 'top',
                align: 'center'
            }
        });
    }

    var docElement = findElementByText('.caption-subject.bold.uppercase.custom-modal', 'DOCUMENT TEMPLATE') ||
        findElementByText('.caption-subject', 'DOCUMENT TEMPLATE');

    if (docElement) {
        steps.push({
            element: docElement,
            popover: {
                title: 'Document Template',
                description: 'Manage sales related entries and master setup configurations here.',
                side: 'top',
                align: 'center'
            }
        });
    }
    var lcElement = findElementByText('.caption-subject.bold.uppercase.custom-modal', 'LC') ||
        findElementByText('.caption-subject', 'LC');
    if (lcElement) {
        steps.push({
            element: lcElement,
            popover: {
                title: 'Lc',
                description: 'Here is LC module.',
                side: 'top',
                align: 'center'
            }
        });
    }

    var planElement = findElementByText('.caption-subject.bold.uppercase.custom-modal', 'PLANNING') ||
        findElementByText('.caption-subject', 'PLANNING');

    if (planElement) {
        steps.push({
            element: planElement,
            popover: {
                title: 'Planning Module',
                description: 'Access all planning related activities and sales plans.',
                side: 'top',
                align: 'center'
            }
        });
    }
    var posElement = findElementByText('.caption-subject.bold.uppercase.custom-modal', 'POS') ||
        findElementByText('.caption-subject', 'POS');

    if (posElement) {
        steps.push({
            element: posElement,
            popover: {
                title: 'POS Module',
                description: 'Access all POS related activities.',
                side: 'top',
                align: 'center'
            }
        });
    }
    var projectElement = findElementByText('.caption-subject.bold.uppercase.custom-modal', 'PROJECT MANAGEMENT') ||
        findElementByText('.caption-subject', 'PROJECT MANAGEMENT');

    if (projectElement) {
        steps.push({
            element: projectElement,
            popover: {
                title: 'Project Management',
                description: 'Access all project management related activities.',
                side: 'top',
                align: 'center'
            }
        });

    }
    var qaqcElement = findElementByText('.caption-subject.bold.uppercase.custom-modal', 'QAQC MANAGEMENT') ||
        findElementByText('.caption-subject', 'QAQC MANAGEMENT');

    if (qaqcElement) {
        steps.push({
            element: qaqcElement,
            popover: {
                title: 'QAQC Module',
                description: 'Access all QAQC related activities.',
                side: 'top',
                align: 'center'
            }
        });
    }
    var quotationElement = findElementByText('.caption-subject.bold.uppercase.custom-modal', 'QUOTATION MANAGEMENT') ||
        findElementByText('.caption-subject', 'QUOTATION MANAGEMENT');

    if (quotationElement) {
        steps.push({
            element: quotationElement,
            popover: {
                title: 'Quotation Module',
                description: 'here is Quatation Management.',
                side: 'top',
                align: 'center'
            }
        });
    }
    var transElement = findElementByText('.caption-subject.bold.uppercase.custom-modal', 'TRANSACTION') ||
        findElementByText('.caption-subject', 'TRANSACTION');

    if (transElement) {
        steps.push({
            element: transElement,
            popover: {
                title: 'Transaction Module',
                description: 'this is transaction module.',
                side: 'top',
                align: 'center'
            }
        });
    }

    if (steps.length === 0) {
        console.warn('No tour steps could be created - elements not found.');
        return;
    }

    var driverObj = driver({
        showProgress: false,
        animate: true,
        allowClose: true,
        overlayClickExit: false,
        stagePadding: 10,
        stageRadius: 5,
        popoverOffset: 10,
        showButtons: ['next', 'previous', 'close'],
        nextBtnText: 'Next →',
        prevBtnText: '← Previous',
        doneBtnText: 'Finish',
        progressText: '{{current}} of {{total}}',
        onDestroyStarted: function () {
            // Remove Enter key listener when tour ends
            document.removeEventListener('keydown', handleEnterKey);

            localStorage.setItem('globalDashboardTourCompleted', 'true');
            driverObj.destroy();
        },
        steps: steps
    });

    // Handler for Enter key navigation
    function handleEnterKey(e) {
        if (e.key === 'Enter') {
            e.preventDefault();
            driverObj.moveNext();
        }
    }

    // Add listener for Enter key
    document.addEventListener('keydown', handleEnterKey);

    driverObj.drive();
}

function restartGlobalDashboardTour() {
    localStorage.removeItem('globalDashboardTourCompleted');
    initializeTour();
}

function resetGlobalDashboardTourState() {
    localStorage.removeItem('globalDashboardTourCompleted');
    console.log('Tour state has been reset. It will start on next page load.');
}
