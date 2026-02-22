const MAX_NOTIFICATIONS = 10;
const STORAGE_KEY = 'news_notifications';
let connection = null;

document.addEventListener("DOMContentLoaded", function () {
    // 1. Initialize UI from LocalStorage
    renderNotifications();

    // 2. Setup SignalR Connection
    // Assuming API_BASE_URL is available, otherwise extract from current host or hardcode according to your setup.
    // Core API is usually running on port 5042 or 5000 based on appsettings.json.
    // We will fetch the config from an endpoint or use an absolute URL.
    // Let's use the current origin if they are on same origin, else we need the backend URL.
    // A safe approach in this project is to use the Proxy or direct hardcoded for now.
    // The Core API url from settings: http://localhost:5042
    
    // Using a relative path won't work if FE and BE are on different ports without proxy.
    // So we hardcode the CoreAPI URL for now, or if you have a GLOBAL var, use it.
    const coreApiUrl = 'http://localhost:5042/hubs/notifications';

    connection = new signalR.HubConnectionBuilder()
        .withUrl(coreApiUrl)
        .withAutomaticReconnect()
        .configureLogging(signalR.LogLevel.Information)
        .build();

    // 3. Define the ReceiveNotification event BEFORE starting
    connection.on("ReceiveNotification", function (data) {
        console.log("ReceiveNotification received:", data);
        const { newsTitle, createdByName, createdDate } = data;
        
        // Ensure data is valid
        if (!newsTitle) return;

        // Display Toast
        const message = `Bài viết mới: <strong>${newsTitle}</strong> vừa được tạo bởi ${createdByName || 'một người dùng'}.`;
        showToast(message, 'info');

        // Add to Local Storage and update UI
        addNotificationToStorage({
            title: newsTitle,
            creator: createdByName,
            date: createdDate || new Date().toISOString(),
            isRead: false
        });
    });

    // 4. Start Connection
    startConnection();

    // 5. Handle clicks to read notifications
    const bellIcon = document.getElementById("notificationDropdown");
    if (bellIcon) {
        bellIcon.addEventListener('click', function() {
            markAllAsRead();
        });
    }
});

async function startConnection() {
    try {
        await connection.start();
        console.log("SignalR Connected to Notification Hub.");
    } catch (err) {
        console.error("SignalR Connection Error: ", err);
        setTimeout(startConnection, 5000); // Retry connection
    }
}

function getStoredNotifications() {
    const data = localStorage.getItem(STORAGE_KEY);
    return data ? JSON.parse(data) : [];
}

function saveNotifications(notifications) {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(notifications));
}

function addNotificationToStorage(notification) {
    let notifications = getStoredNotifications();
    
    // Add to beginning
    notifications.unshift(notification);
    
    // Keep only the latest 10
    if (notifications.length > MAX_NOTIFICATIONS) {
        notifications = notifications.slice(0, MAX_NOTIFICATIONS);
    }
    
    saveNotifications(notifications);
    renderNotifications();
}

function markAllAsRead() {
    let notifications = getStoredNotifications();
    let changed = false;
    
    notifications.forEach(n => {
        if (!n.isRead) {
            n.isRead = true;
            changed = true;
        }
    });

    if (changed) {
        saveNotifications(notifications);
        // We delay rendering badges slightly to let user see them before disappearing
        setTimeout(renderNotifications, 1000);
    }
}

function renderNotifications() {
    const list = document.getElementById("notificationList");
    const badge = document.getElementById("notificationBadge");
    const countHeader = document.getElementById("notificationCountHeader");
    const noItem = document.getElementById("noNotificationItem");
    
    if (!list || !badge) return;

    let notifications = getStoredNotifications();
    
    // Calculate Unread
    const unreadCount = notifications.filter(n => !n.isRead).length;
    
    // Update Badge
    if (unreadCount > 0) {
        badge.textContent = unreadCount;
        badge.style.display = 'inline-block';
    } else {
        badge.style.display = 'none';
        badge.textContent = '0';
    }

    if (countHeader) countHeader.textContent = notifications.length;

    // Clear existing dynamic items
    const existingItems = list.querySelectorAll('.dyn-notif');
    existingItems.forEach(item => item.remove());

    if (notifications.length === 0) {
        if (noItem) noItem.style.display = 'block';
    } else {
        if (noItem) noItem.style.display = 'none';
        
        // Append items in order
        notifications.forEach(n => {
            const dateStr = new Date(n.date).toLocaleString('vi-VN');
            const bgClass = n.isRead ? '' : 'bg-light';
            const fwClass = n.isRead ? '' : 'fw-bold';
            
            const li = document.createElement('li');
            li.className = 'dyn-notif';
            li.innerHTML = `
                <a class="dropdown-item ${bgClass} border-bottom py-2" href="#" style="white-space: normal;">
                    <div class="d-flex justify-content-between w-100">
                        <small class="text-primary mb-1"><i class="bi bi-newspaper"></i> Bài viết mới</small>
                        <small class="text-muted" style="font-size: 0.75em;">${dateStr}</small>
                    </div>
                    <p class="mb-1 ${fwClass}" style="font-size: 0.9em; word-wrap: break-word;">${n.title}</p>
                    <small class="text-muted" style="font-size: 0.8em;">Đăng bởi: ${n.creator || 'Staff'}</small>
                </a>
            `;
            list.appendChild(li);
        });
    }
}
