function toggleSidebar() {
    const sidebar = document.getElementById('sidebar');
    const overlay = document.getElementById('overlay');
    
    if (sidebar) {
        sidebar.classList.toggle('collapsed');
    }
    
    // Toggle overlay on mobile
    if (overlay && window.innerWidth <= 768) {
        overlay.classList.toggle('active');
    }
}

function toggleSubmenu(event, submenuId) {
    event.stopPropagation();
    const submenu = document.getElementById(submenuId);
    const arrow = event.currentTarget.querySelector('.submenu-arrow');
    
    if (submenu) {
        submenu.classList.toggle('expanded');
    }
    if (arrow) {
        arrow.classList.toggle('expanded');
    }
}

// function toggleUserDropdown() {
//     const dropdown = document.getElementById('userDropdownMenu');
//     const arrow = document.querySelector('.dropdown-arrow');
    
//     if (dropdown) {
//         dropdown.classList.toggle('open');
//     }
//     if (arrow) {
//         arrow.classList.toggle('open');
//     }
// }

// Wait for DOM to be fully loaded
document.addEventListener('DOMContentLoaded', function() {
    // Close dropdown when clicking outside
    // document.addEventListener('click', function(event) {
    //     const dropdown = document.querySelector('.user-dropdown');
    //     const dropdownMenu = document.getElementById('userDropdownMenu');
    //     const arrow = document.querySelector('.dropdown-arrow');
        
    //     if (dropdown && dropdownMenu && arrow && !dropdown.contains(event.target)) {
    //         dropdownMenu.classList.remove('open');
    //         arrow.classList.remove('open');
    //     }
    // });

    // Close sidebar when clicking overlay
    const overlay = document.getElementById('overlay');
    if (overlay) {
        overlay.addEventListener('click', function() {
            toggleSidebar();
        });
    }

    // Handle responsive behavior
    window.addEventListener('resize', function() {
        const sidebar = document.getElementById('sidebar');
        const overlay = document.getElementById('overlay');
        
        if (sidebar && overlay && window.innerWidth > 768) {
            sidebar.classList.remove('collapsed');
            overlay.classList.remove('active');
        }
    });
});


// AI assistant


// Add click handlers for suggestion buttons
const suggestionBtns = document.querySelectorAll('.suggestion-btn');
const messageInput = document.querySelector('.message-input');

suggestionBtns.forEach(btn => {
    btn.addEventListener('click', () => {
        messageInput.value = btn.textContent;
        messageInput.focus();
    });
});

// Add focus effect to input
const inputWrapper = document.querySelector('.input-wrapper');
messageInput.addEventListener('focus', () => {
    inputWrapper.style.borderColor = '#a3a3a3';
});

messageInput.addEventListener('blur', () => {
    if (!messageInput.value) {
        inputWrapper.style.borderColor = '#d4d4d4';
    }
});

// Add Enter key handler
messageInput.addEventListener('keypress', (e) => {
    if (e.key === 'Enter' && messageInput.value.trim()) {
        console.log('Sending message:', messageInput.value);
        // Here you would handle the message submission
        messageInput.value = '';
    }
});