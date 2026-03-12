(function () {
    "use strict";

    let modalOpen = false;

    window.addEventListener("keydown", function (e) {
        const key = e.key.toLowerCase();        
        if (modalOpen) return;


        if (e.altKey) {
            const isShift = e.shiftKey;
            const isCtrl = e.ctrlKey;

            if (isShift && !isCtrl) {
                switch (key) {
                    case "c":
                        e.preventDefault();
                        openModal("/DocumentTemplate/Home/Index#!DT/CompanySetup", "Company Setup");
                        break;
                    case "b":
                        e.preventDefault();
                        openModal("/DocumentTemplate/Home/Index#!DT/BranchSetup", "Branch Setup");
                        break;
                }
            }
            else if (isCtrl && !isShift) {
                switch (key) {
                    case "c":
                        e.preventDefault();
                        openModal("/DocumentTemplate/Home/Index#!DT/CategorySetup", "Category Setup");
                        break;
                }
            }
            else if (!isShift && !isCtrl) {
                switch (key) {
                    case "c":
                        e.preventDefault();
                        openModal("/DocumentTemplate/Home/Index#!DT/CustomerSetup", "Customer Setup");
                        break;
                    case "i":
                        e.preventDefault();
                        openModal("/DocumentTemplate/Home/Index#!DT/ItemSetup", "Item Setup");
                        break;
                    case "a":
                        e.preventDefault();
                        openModal("/DocumentTemplate/Home/Index#!DT/AccountSetup", "Chart of Accounts Setup");
                        break;
                    case "s":
                        e.preventDefault();
                        openModal("/DocumentTemplate/Home/Index#!DT/SupplierSetup", "Supplier Setup");
                        break;
                }
            }
        }
    });

    function openModal(url, title) {
       
        if (modalOpen) return;
        modalOpen = true;

        const overlay = document.createElement("div");
        Object.assign(overlay.style, {
            position: "fixed", top: "0", left: "0", width: "100%", height: "100%",
            backgroundColor: "rgba(0, 0, 0, 0.8)", zIndex: "100000",
            display: "flex", justifyContent: "center", alignItems: "center"
        });

        const modal = document.createElement("div");
        Object.assign(modal.style, {
            backgroundColor: "#fff", width: "85%", height: "90%",
            borderRadius: "4px", display: "flex", flexDirection: "column",
            boxShadow: "0 20px 60px rgba(0,0,0,0.5)", overflow: "hidden"
        });

        modal.innerHTML = `
            <div style="padding: 10px 20px; border-bottom: 1px solid #ddd; display: flex; justify-content: space-between; align-items: center; background: #3c8dbc; color: white;">
                <h4 style="margin: 0; font-family: 'Open Sans', sans-serif; font-size: 16px;">${title}</h4>
                <button id="modal-close-x" style="cursor: pointer; background: none; border: none; font-size: 24px; color: white;">&times;</button>
            </div>
            <div style="flex: 1; position: relative;">
                <iframe id="modal-iframe" src="${url}" style="width: 100%; height: 100%; border: none;"></iframe>
            </div>
        `;

        overlay.appendChild(modal);
        document.body.appendChild(overlay);

        const iframe = document.getElementById("modal-iframe");

        overlay.addEventListener("click", function (e) {
            if (e.target === overlay) close();
        });

        iframe.onload = function () {
            try {
                const doc = iframe.contentDocument || iframe.contentWindow.document;
                const style = doc.createElement('style');
                style.textContent = `
                    #documentSidebar, #documentnavBar, .page-footer, .busy-loader { display: none !important; }
                    .page-container { margin-top: 0 !important; padding: 0 !important; }
                    .page-content-wrapper .page-content { margin-left: 0 !important; margin-top: 0 !important; padding: 15px !important; min-height: 100vh !important; background-color: #fff !important; }
                    #pageContent {margin-left: 0 !important;}
                    body.page-header-fixed { padding-top: 0 !important; }
                    body { background: #fff !important; overflow: auto !important; }
                    #splitter { border: 0 !important; height: calc(100vh - 20px) !important; }
                `;
                doc.head.appendChild(style);
                iframe.contentWindow.dispatchEvent(new Event('resize'));
            } catch (e) {
                console.error("Iframe CSS injection failed.", e);
            }
        };

        const close = () => {
            if (document.body.contains(overlay)) {
                document.body.removeChild(overlay);
            }
            
            modalOpen = false;
            window.removeEventListener("keydown", escListener);
        };

        document.getElementById("modal-close-x").onclick = close;

        const escListener = (e) => {
            if (e.key === "Escape") close();
        };
        window.addEventListener("keydown", escListener);
    }
})();