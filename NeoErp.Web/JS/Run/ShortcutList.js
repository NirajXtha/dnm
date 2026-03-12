/**
 * System Shortcuts Modal (Professional Header Version)
 * Press Ctrl + Shift + H to toggle
 */
(function () {
    const initShortcuts = () => {
        const shortcuts = [
            { keys: "Alt + C", action: "Customer Setup" },
            { keys: "Alt + I", action: "Items Setup" },
            { keys: "Alt + A", action: "Chart of Accounts Setup" },
            { keys: "Alt + S", action: "Supplier Setup" },
            { keys: "Alt + Shift + C", action: "Company Setup" },
            { keys: "Alt + Shift + B", action: "Branch Setup" },
            { keys: "Alt + Ctrl + C", action: "Category Setup" }
        ];

        const style = document.createElement('style');
        style.textContent = `
            #shortcut-modal-overlay {
                display: none;
                position: fixed;
                top: 0; left: 0; 
                width: 100%; height: 100%;
                background: rgba(15, 23, 42, 0.8); /* Modern Slate Blue Overlay */
                z-index: 999999;
                justify-content: center;
                align-items: center;
                font-family: 'Inter', -apple-system, system-ui, sans-serif;
                cursor: pointer;
                backdrop-filter: blur(4px); /* Frosty glass effect */
            }
            .shortcut-card {
                background: white;
                border-radius: 12px;
                width: 90%;
                max-width: 500px;
                max-height: 75vh;
                display: flex;
                flex-direction: column;
                box-shadow: 0 25px 50px -12px rgba(0, 0, 0, 0.5);
                cursor: default;
                animation: modalSlideUp 0.3s cubic-bezier(0.34, 1.56, 0.64, 1);
                overflow: hidden;
            }
            @keyframes modalSlideUp {
                from { opacity: 0; transform: translateY(30px) scale(0.95); }
                to { opacity: 1; transform: translateY(0) scale(1); }
            }
            
            /* --- IMPROVED HEADER --- */
            .shortcut-header {
                display: flex;
                align-items: center;
                justify-content: space-between;
                padding: 16px 24px;
                background: #f8fafc;
                border-bottom: 1px solid #e2e8f0;
                flex-shrink: 0;
            }
            .shortcut-header-title {
                margin: 0;
                font-size: 1.1rem;
                font-weight: 700;
                color: #1e293b;
                display: flex;
                align-items: center;
                gap: 8px;
            }
            .shortcut-header-title::before {
                content: '';
                display: inline-block;
                width: 4px;
                height: 18px;
                background: #3b82f6; /* Accent color */
                border-radius: 2px;
            }
            .shortcut-close-btn {
                background: none;
                border: none;
                color: #94a3b8;
                font-size: 24px;
                cursor: pointer;
                line-height: 1;
                padding: 4px;
                transition: color 0.2s, transform 0.2s;
            }
            .shortcut-close-btn:hover {
                color: #ef4444;
                transform: rotate(90deg);
            }

            .shortcut-list-container {
                padding: 8px 24px;
                overflow-y: auto;
                flex-grow: 1;
                background: #ffffff;
            }
            .shortcut-table {
                width: 100%;
                border-collapse: collapse;
            }
            .shortcut-table tr:last-child td {
                border-bottom: none;
            }
            .shortcut-table td {
                padding: 16px 0;
                border-bottom: 1px solid #f1f5f9;
            }
            .kbd-style {
                background: #ffffff;
                border: 1px solid #cbd5e1;
                border-bottom: 3px solid #cbd5e1; /* 3D effect */
                border-radius: 6px;
                padding: 4px 10px;
                font-family: ui-monospace, monospace;
                font-weight: 700;
                color: #334155;
                font-size: 11px;
                text-transform: uppercase;
            }
            .action-text {
                color: #475569;
                font-weight: 500;
                font-size: 1.4rem !important;
                text-align: right;
            }
            .shortcut-footer {
                padding: 12px 24px;
                text-align: center;
                font-size: 11px;
                color: #64748b;
                background: #f8fafc;
                border-top: 1px solid #e2e8f0;
                flex-shrink: 0;
            }
        `;
        document.head.appendChild(style);

        const overlay = document.createElement('div');
        overlay.id = 'shortcut-modal-overlay';

        const tableRows = shortcuts.map(s => `
            <tr>
                <td><span class="kbd-style">${s.keys}</span></td>
                <td class="action-text">${s.action}</td>
            </tr>
        `).join('');

        overlay.innerHTML = `
            <div class="shortcut-card">
                <div class="shortcut-header">
                    <h2 class="shortcut-header-title">Keyboard Shortcuts</h2>
                    <button class="shortcut-close-btn" id="close-shortcuts-btn">&times;</button>
                </div>
                <div class="shortcut-list-container">
                    <table class="shortcut-table">
                        <tbody>${tableRows}</tbody>
                    </table>
                </div>
                <div class="shortcut-footer">
                    Press <b>ESC</b> or click outside to dismiss
                </div>
            </div>
        `;

        document.body.appendChild(overlay);

        const toggleModal = (show) => {
            overlay.style.display = show ? 'flex' : 'none';
        };

        document.addEventListener('keydown', (e) => {
            if (e.ctrlKey && e.shiftKey && e.code === 'KeyH') {
                e.preventDefault();
                toggleModal(overlay.style.display !== 'flex');
            }
            if (e.key === 'Escape' && overlay.style.display === 'flex') {
                toggleModal(false);
            }
        });

        overlay.addEventListener('click', (e) => {
            if (e.target === overlay) toggleModal(false);
        });

        document.getElementById('close-shortcuts-btn').addEventListener('click', () => {
            toggleModal(false);
        });
    };

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", initShortcuts);
    } else {
        initShortcuts();
    }
})();