// MenuItem.js
//document.addEventListener('DOMContentLoaded', function () {
//    'use strict';

//    let menuList = [];

//    function loadMenus() {
//        fetch('/api/MenuSettings/GetMenuList')
//            .then(res => res.json())
//            .then(data => {
//                menuList = data || [];
//            })
//            .catch(err => {
//                console.error('Menu API Error:', err);
//            });
//    }

//    const template = `
//        <div id="qm-overlay" style="display:none">
//            <div id="qm-popup">
//                <div id="qm-header">
//                    <span>Quick Menu (Ctrl + Space)</span>
//                    <button id="qm-close">&times;</button>
//                </div>
//                <input id="qm-search" type="text" placeholder="Search menu..." />
//                <ul id="qm-list"></ul>
//            </div>
//        </div>
//    `;
//    document.body.insertAdjacentHTML('beforeend', template);

//    const css = `
//        #qm-overlay { position: fixed; inset: 0; background: rgba(0,0,0,0.35); z-index: 9999; }
//        #qm-popup { width: 420px; background: #fff; margin: 120px auto; border-radius: 4px; box-shadow: 0 6px 25px rgba(0,0,0,0.3); overflow: hidden; }
//        #qm-header { padding: 10px 12px; font-weight: bold; display: flex; justify-content: space-between; align-items: center; border-bottom: 1px solid #ddd; }
//        #qm-header button { border: none; background: none; font-size: 22px; cursor: pointer; }
//        #qm-search { width: 100%; padding: 10px; border: none; border-bottom: 1px solid #ddd; outline: none; font-size: 14px; }
//        #qm-list { list-style: none; margin: 0; padding: 0; max-height: 300px; overflow-y: auto; }
//        #qm-list li { padding: 8px 12px; cursor: pointer; border-bottom: 1px solid #eee; }
//        #qm-list li:hover { background: #f3f3f3; }
//    `;
//    const style = document.createElement('style');
//    style.innerHTML = css;
//    document.head.appendChild(style);

//    const overlay = document.getElementById('qm-overlay');
//    const searchInput = document.getElementById('qm-search');
//    const listEl = document.getElementById('qm-list');
//    const closeBtn = document.getElementById('qm-close');

//    function openPopup() {
//        overlay.style.display = 'block';
//        searchInput.value = '';
//        renderList(menuList);
//        setTimeout(() => searchInput.focus(), 50);
//    }

//    function closePopup() {
//        overlay.style.display = 'none';
//    }

//    function renderList(data) {
//        listEl.innerHTML = '';
//        data.forEach(menu => {
//            const li = document.createElement('li');
//            li.innerHTML = `<strong>${menu.MENU_EDESC}</strong> <small style="color: #888; float: right;">${menu.MENU_NO}</small>`;
//            li.addEventListener('click', function () {
//                navigate(menu.VIRTUAL_PATH);
//            });
//            listEl.appendChild(li);
//        });
//    }

//    /************************************
//     * FIX: NAVIGATION LOGIC
//     ************************************/
//    function navigate(path) {
//        closePopup();
//        if (!path) return;

//        // Ensure the path starts with / to avoid appending to the current URL
//        const targetPath = path.startsWith('/') ? path : '/' + path;

//        // Use href to redirect the whole page to the new location
//        window.location.href = targetPath;
//    }

//    overlay.addEventListener('click', function (e) {
//        if (e.target === overlay) closePopup();
//    });

//    document.addEventListener('keydown', function (e) {
//        if (e.ctrlKey && e.code === 'Space') {
//            e.preventDefault();
//            openPopup();
//        }
//        if (e.key === 'Escape') {
//            closePopup();
//        }
//    });

//    closeBtn.addEventListener('click', closePopup);

//    searchInput.addEventListener('input', function () {
//        const text = this.value.toLowerCase();
//        const filtered = menuList.filter(m =>
//            (m.MENU_EDESC && m.MENU_EDESC.toLowerCase().includes(text)) ||
//            (m.MENU_NO && m.MENU_NO.toString().toLowerCase().includes(text))
//        );
//        renderList(filtered);
//    });

//    loadMenus();
//});




// MenuItem.js
document.addEventListener('DOMContentLoaded', function () {
    'use strict';

    let menuList = [];

    // 1. Initial Data Fetch
    function loadMenus() {
        fetch('/api/MenuSettings/GetMenuList')
            .then(res => res.json())
            .then(data => {
                menuList = data || [];
            })
            .catch(err => {
                console.error('Menu API Error:', err);
            });
    }

    // 2. Inject HTML Template
    const template = `
        <div id="qm-overlay" style="display:none">
            <div id="qm-popup">
                <div id="qm-header">
                    <span>Quick Menu (Ctrl + Space)</span>
                    <button id="qm-close">&times;</button>
                </div>
                <input id="qm-search" type="text" placeholder="Type to search menu..." autocomplete="off" />
                <ul id="qm-list"></ul>
            </div>
        </div>
    `;
    document.body.insertAdjacentHTML('beforeend', template);

    // 3. Inject CSS
    const css = `
        #qm-overlay { position: fixed; inset: 0; background: rgba(0,0,0,0.4); z-index: 9999; backdrop-filter: blur(2px); }
        #qm-popup { width: 450px; background: #fff; margin: 100px auto; border-radius: 8px; box-shadow: 0 10px 30px rgba(0,0,0,0.3); overflow: hidden; font-family: sans-serif; }
        #qm-header { padding: 12px 15px; background: #f8f9fa; font-weight: bold; display: flex; justify-content: space-between; align-items: center; border-bottom: 1px solid #eee; color: #333; }
        #qm-header button { border: none; background: none; font-size: 24px; cursor: pointer; color: #999; line-height: 1; }
        #qm-header button:hover { color: #333; }
        #qm-search { width: 100%; padding: 14px; border: none; border-bottom: 1px solid #eee; outline: none; font-size: 16px; box-sizing: border-box; }
        #qm-list { list-style: none; margin: 0; padding: 0; max-height: 350px; overflow-y: auto; }
        #qm-list li { padding: 10px 15px; cursor: pointer; border-bottom: 1px solid #f9f9f9; transition: background 0.2s; }
        #qm-list li:hover { background: #f0f7ff; }
        .qm-no-result { padding: 20px; text-align: center; color: #888; font-style: italic; }
    `;
    const style = document.createElement('style');
    style.innerHTML = css;
    document.head.appendChild(style);

    // 4. Elements & State Management
    const overlay = document.getElementById('qm-overlay');
    const searchInput = document.getElementById('qm-search');
    const listEl = document.getElementById('qm-list');
    const closeBtn = document.getElementById('qm-close');

    function openPopup() {
        overlay.style.display = 'block';
        searchInput.value = '';
        listEl.innerHTML = ''; // Ensure list is empty on open
        setTimeout(() => searchInput.focus(), 50);
    }

    function closePopup() {
        overlay.style.display = 'none';
    }

    // 5. Render Logic (Modified for search-only visibility)
    function renderList(data) {
        const query = searchInput.value.trim().toLowerCase();
        listEl.innerHTML = '';

        // If search is empty, don't show anything
        if (query === "") return;

        // If no results found
        if (data.length === 0) {
            listEl.innerHTML = `<li class="qm-no-result">No menus match "${query}"</li>`;
            return;
        }

        data.forEach(menu => {
            const li = document.createElement('li');
            li.innerHTML = `
                <div style="display: flex; justify-content: space-between; align-items: center;">
                    <span style="color: #333; font-weight: 500;">${menu.MENU_EDESC}</span>
                    <span style="font-size: 11px; color: #999; background: #f0f0f0; padding: 2px 6px; border-radius: 4px;">${menu.MENU_NO}</span>
                </div>
            `;
            li.addEventListener('click', function () {
                navigate(menu.VIRTUAL_PATH);
            });
            listEl.appendChild(li);
        });
    }

    function navigate(path) {
        closePopup();
        if (!path) return;
        const targetPath = path.startsWith('/') ? path : '/' + path;
        window.location.href = targetPath;
    }

    overlay.addEventListener('click', function (e) {
        if (e.target === overlay) closePopup();
    });

    // 6. Event Listeners
    document.addEventListener('keydown', function (e) {
        // Ctrl + Space to toggle
        if (e.ctrlKey && e.code === 'Space') {
            e.preventDefault();
            openPopup();
        }
        // Escape to close
        if (e.key === 'Escape') {
            closePopup();
        }
    });

    closeBtn.addEventListener('click', closePopup);

    // Close when clicking outside the popup
    overlay.addEventListener('click', function (e) {
        if (e.target === overlay) closePopup();
    });

    searchInput.addEventListener('input', function () {
        const text = this.value.toLowerCase().trim();

        if (text === "") {
            listEl.innerHTML = '';
            return;
        }

        const filtered = menuList.filter(m =>
            (m.MENU_EDESC && m.MENU_EDESC.toLowerCase().includes(text)) ||
            (m.MENU_NO && m.MENU_NO.toString().toLowerCase().includes(text))
        );

        renderList(filtered);
    });

    // 7. Initialize
    loadMenus();
});