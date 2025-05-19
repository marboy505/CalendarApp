<template>
  <div :class="{'w-64': !isSidebarCollapsed, 'w-16': isSidebarCollapsed}" class="bg-gray-800 text-white p-4 h-full transition-all">
    <!-- Sidebar Toggle Button (Hamburger) -->
    <button @click="toggleSidebar" class="text-white mb-4 lg:hidden absolute top-4 left-4">
      <span v-if="isSidebarCollapsed">
        ☰ <!-- Hamburger icon when collapsed -->
      </span>
      <span v-else>
        ✖ <!-- Close icon when expanded -->
      </span>
    </button>

    <!-- Sidebar Header / User Profile -->
    <div v-if="!isSidebarCollapsed" class="flex items-center mb-6">
      <img v-if="userProfileImage" :src="userProfileImage" alt="User Profile" class="w-10 h-10 rounded-full mr-3"/>
      <div>
        <p class="text-lg font-bold">{{ userName }}</p>
        <p class="text-sm text-gray-400">{{ userRole }}</p>
      </div>
    </div>

    <!-- Sidebar Navigation Links -->
    <ul>
      <!-- Dashboard -->
      <li>
        <router-link to="/" class="block py-2 hover:bg-gray-700 rounded transition-all" :class="{'bg-gray-600': isActive('/') }">
          <i class="fas fa-tachometer-alt mr-2" v-if="!isSidebarCollapsed"></i> 
          <span v-if="!isSidebarCollapsed">Dashboard</span>
        </router-link>
      </li>

      <!-- Admin Panel (Only for Admins) -->
      <li v-if="isAdmin">
        <router-link to="/admin" class="block py-2 hover:bg-gray-700 rounded transition-all" :class="{'bg-gray-600': isActive('/admin') }">
          <i class="fas fa-cogs mr-2" v-if="!isSidebarCollapsed"></i> 
          <span v-if="!isSidebarCollapsed">Admin Panel</span>
        </router-link>
      </li>

      <!-- Calendar -->
      <li>
        <router-link to="/calendar" class="block py-2 hover:bg-gray-700 rounded transition-all" :class="{'bg-gray-600': isActive('/calendar') }">
          <i class="fas fa-calendar-alt mr-2" v-if="!isSidebarCollapsed"></i> 
          <span v-if="!isSidebarCollapsed">Calendar</span>
        </router-link>
      </li>

      <!-- Event List -->
      <li>
        <router-link to="/event-list" class="block py-2 hover:bg-gray-700 rounded transition-all" :class="{'bg-gray-600': isActive('/event-list') }">
          <i class="fas fa-list-alt mr-2" v-if="!isSidebarCollapsed"></i> 
          <span v-if="!isSidebarCollapsed">Event List</span>
        </router-link>
      </li>

      <!-- Message -->
      <li>
        <router-link to="/waMassage" class="block py-2 hover:bg-gray-700 rounded transition-all" :class="{'bg-gray-600': isActive('/event-list') }">
          <i class="fas fa-comment mr-2" v-if="!isSidebarCollapsed"></i> 
          <span v-if="!isSidebarCollapsed">Message</span>
        </router-link>
      </li>
    </ul>

    <!-- Dark Mode Toggle -->
    <button @click="toggleDarkMode" class="text-white mt-4 mb-4">
      <span v-if="isDarkMode">🌙</span>
      <span v-else>🌞</span> <!-- Dark/Light Mode Toggle Icon -->
    </button>
  </div>
</template>

<script>
export default {
  data() {
    return {
      isSidebarCollapsed: false, // Sidebar status (collapsed or expanded)
      activeRoute: this.$route.path, // Track active route for styling
      isAdmin: false, // Check if the user is an admin
      userName: 'John Doe', // User name
      userRole: 'Admin', // User role
      userProfileImage: 'https://www.w3schools.com/w3images/avatar2.png', // User profile image URL
      isDarkMode: false, // Dark mode status
    };
  },
  methods: {
    // Toggle Sidebar collapse
    toggleSidebar() {
      this.isSidebarCollapsed = !this.isSidebarCollapsed;
    },
    // Check if the current route is active for highlighting menu items
    isActive(route) {
      return this.activeRoute === route;
    },
    // Toggle Dark Mode
    toggleDarkMode() {
      this.isDarkMode = !this.isDarkMode;
      // Save dark mode preference to localStorage
      localStorage.setItem('darkMode', this.isDarkMode);
      // Emit dark mode status change to parent (App.vue)
      this.$emit('toggleDarkMode', this.isDarkMode);
    }
  },
  created() {
    // Retrieve dark mode status from localStorage
    const savedDarkMode = localStorage.getItem('darkMode');
    if (savedDarkMode !== null) {
      this.isDarkMode = JSON.parse(savedDarkMode);
    }
    const userRole = localStorage.getItem('role') || 'user'; // Get user role from localStorage
    this.isAdmin = userRole === 'admin'; // Check if the user is an admin
  },
  watch: {
    '$route.path'(newPath) {
      this.activeRoute = newPath;
    }
  }
};
</script>

<style scoped>
.transition-all {
  transition: width 0.3s ease;
}

ul {
  list-style-type: none;
  padding: 0;
}

li {
  margin: 10px 0;
}

router-link {
  display: block;
  padding: 8px 12px;
  text-decoration: none;
  color: white;
  border-radius: 4px;
}

router-link:hover {
  background-color: #4b5563;
}

.bg-gray-600 {
  background-color: #4b5563;
}

.bg-gray-800 {
  background-color: #2d3748;
}

.bg-gray-700 {
  background-color: #4a5568;
}

.rounded {
  border-radius: 8px;
}

.fas {
  font-size: 18px;
}

button {
  margin-top: 10px;
}

/* Responsif pada perangkat kecil */
@media (max-width: 768px) {
  .lg\\:hidden {
    display: block !important;
  }
  .w-64 {
    width: 100% !important;
  }
  .w-16 {
    width: 0 !important;
  }
  .p-4 {
    padding: 0 !important;
  }
  .h-full {
    height: 100% !important;
  }
}
</style>
