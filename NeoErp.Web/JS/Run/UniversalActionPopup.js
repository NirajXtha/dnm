(function () {

    // Main function to show popup
    window.showUniversalPopup = function (options) {

        var settings = {
            title: options.title || "Confirmation",
            message: options.message || "Are you sure?",
            confirmText: options.confirmText || "Confirm",
            confirmColor: options.confirmColor || null, // optional
            cancelText: options.cancelText || "Cancel",
            steps: options.steps || null, // array of HTML strings for multi-step
            onConfirm: options.onConfirm || function () { },
            onCancel: options.onCancel || function () { }
        };

        // Remove existing popup if exists
        var existing = document.getElementById("uPopup");
        if (existing) existing.remove();

        // Inject CSS
        var style = document.createElement("style");
        style.innerHTML = `
        .u-overlay {
            position: fixed;
            inset: 0;
            background: rgba(0,0,0,0.5);
            display: flex;
            justify-content: center;
            align-items: flex-start; /* align top */
            z-index: 9999;
            padding: 20px 10px 10px 10px; /* add top padding so modal isn't at absolute top */
            overflow-y: auto; /* allows scrolling if popup is taller than viewport */
        }

        .u-box {
            background: #fff;
            width: 90%;
            max-width: 400px;
            max-height: 80vh;
            border-radius: 8px;
            display: flex;
            flex-direction: column;
            box-shadow: 0 8px 20px rgba(0,0,0,0.2);
            animation: fadeIn 0.2s ease-in-out;
        }
        /* Title with green background */
        .u-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            padding: 9px;
            font-weight: bold;
            font-size: 13px;
            background-color: #28a745; /* Green */
            color: #fff; /* White text */
            border-top-left-radius: 8px;
            border-top-right-radius: 8px;
        }
        .u-close-btn {
            background: none;
            border: none;
            color: white; /* Your requested color */
            font-size: 19px;
            font-weight: bold;
            cursor: pointer;
            line-height: 1;
            padding: 0;
            margin-left: 10px;
         }
        
        .u-close-btn:hover {
            opacity: 0.7;
        }
        .u-body {
            padding: 12px;
            overflow-y: auto;
            font-size: 13px;
            color: #000; /* Force black text */
            min-height: 70px;
            word-break: break-word;
         }

        .u-footer {
            padding: 10px;
            border-top: 1px solid #eee;
            display: flex;
            justify-content: space-between;
            flex-wrap: wrap;
            gap: 5px;
        }
        .u-btn {
            border: none;
            padding: 6px 12px;
            border-radius: 4px;
            cursor: pointer;
            font-size: 12px;
            color: white;
            transition: background 0.2s, transform 0.1s;
        }
        .u-btn:hover {
            filter: brightness(110%);
            transform: scale(1.05);
        }
        .u-cancel { background: #6c757d; }
        .u-confirm { background: #dc3545; }
        .u-nav { background: #007bff; }
        .u-hidden { display: none !important; }

        @keyframes fadeIn {
            from { transform: scale(0.9); opacity: 0; }
            to { transform: scale(1); opacity: 1; }
        }
        `;
        document.head.appendChild(style);

        // Create overlay
        var overlay = document.createElement("div");
        overlay.id = "uPopup";
        overlay.className = "u-overlay";

        overlay.innerHTML = `
    <div class="u-box">
        <div class="u-header">
            <span id="uTitle">${settings.title}</span>
            <button id="uClose" class="u-close-btn">&times;</button>
        </div>
        <div class="u-body" id="uBody"></div>
        <div class="u-footer">
            <div>
                <button id="uPrev" class="u-btn u-nav u-hidden">Back</button>
                <button id="uNext" class="u-btn u-nav u-hidden">Next</button>
            </div>
            <div>
                <button id="uCancel" class="u-btn u-cancel">${settings.cancelText}</button>
                <button id="uConfirm" class="u-btn u-confirm">${settings.confirmText}</button>
            </div>
        </div>
    </div>
`;
        document.body.appendChild(overlay);

        // Elements
        var bodyEl = document.getElementById("uBody");
        var btnNext = document.getElementById("uNext");
        var btnPrev = document.getElementById("uPrev");
        var btnConfirm = document.getElementById("uConfirm");
        var btnCancel = document.getElementById("uCancel");
        var btnClose = document.getElementById("uClose");

        // Steps logic
        var pages = settings.steps || splitMessage(settings.message);
        var currentIndex = 0;

        function splitMessage(msg) {
            if (Array.isArray(msg)) return msg;
            if (msg.length > 300) {
                return msg.match(/.{1,300}/g) || [msg];
            }
            return [msg];
        }

        function updateBody() {
            bodyEl.innerHTML = pages[currentIndex];
            if (pages.length > 1) {
                btnPrev.classList.toggle("u-hidden", currentIndex === 0);
                btnNext.classList.toggle("u-hidden", currentIndex === pages.length - 1);
            } else {
                btnPrev.classList.add("u-hidden");
                btnNext.classList.add("u-hidden");
            }
        }

        updateBody();

        // Buttons
        btnNext.onclick = function () {
            if (currentIndex < pages.length - 1) {
                currentIndex++;
                updateBody();
            }
        };
        btnPrev.onclick = function () {
            if (currentIndex > 0) {
                currentIndex--;
                updateBody();
            }
        };

        // Confirm button color fallback
        if (settings.confirmColor && settings.confirmColor.trim() !== "") {
            btnConfirm.style.backgroundColor = settings.confirmColor;
        } else {
            btnConfirm.style.backgroundColor = ""; // CSS default
        }

        btnConfirm.onclick = function () {
            overlay.remove();
            settings.onConfirm();
        };

        btnCancel.onclick = function () {
            overlay.remove();
            settings.onCancel();
        };
        btnClose.onclick = function () {
            overlay.remove();
            settings.onCancel();
        };

        //overlay.onclick = function (e) {
        //    if (e.target === overlay) {
        //        overlay.remove();
        //    }
        //};
    };

})();
