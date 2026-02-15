// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

window.showToast = function(message, type = 'info') {
    const toastContainer = document.getElementById('toastContainer');
    if (!toastContainer) {
        console.warn('Toast container not found');
        alert(message); // Fallback
        return;
    }

    const toastId = 'toast-' + Date.now();
    // Map types to Bootstrap colors
    const bgClass = type === 'success' ? 'bg-success' : type === 'danger' ? 'bg-danger' : 'bg-info';
    const textClass = 'text-white';
    
    // Icon mapping
    let icon = '';
    if (type === 'success') icon = '<i class="bi bi-check-circle-fill me-2"></i>';
    else if (type === 'danger') icon = '<i class="bi bi-exclamation-triangle-fill me-2"></i>';
    else icon = '<i class="bi bi-info-circle-fill me-2"></i>';

    const toastHtml = `
        <div id="${toastId}" class="toast align-items-center ${bgClass} ${textClass} border-0 mb-2" role="alert" aria-live="assertive" aria-atomic="true">
            <div class="d-flex">
                <div class="toast-body">
                    ${icon}${message}
                </div>
                <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
            </div>
        </div>
    `;

    // Create element from HTML string
    const template = document.createElement('template');
    template.innerHTML = toastHtml.trim();
    const toastElement = template.content.firstChild;

    toastContainer.appendChild(toastElement);

    // Initialize Bootstrap Toast
    const toast = new bootstrap.Toast(toastElement, { delay: 3000 });
    toast.show();

    // Cleanup after hidden
    toastElement.addEventListener('hidden.bs.toast', () => {
        toastElement.remove();
    });
};
