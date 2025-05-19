<template>
  <div class="flex h-full">
    <!-- Sidebar -->
    <Sidebar :isSidebarCollapsed="isSidebarCollapsed" @toggleSidebar="toggleSidebar"/>

    <!-- Main Content -->
    <div class="flex-1 overflow-auto p-6 transition-all" :class="{'ml-16': !isSidebarCollapsed, 'ml-4': isSidebarCollapsed}">
      <router-view /> <!-- Main content that changes based on routes -->
    </div>
  </div>
</template>

<script>
import Sidebar from './components/Sidebar.vue'; // Import Sidebar component for the layout

export default {
  name: 'App',
  components: {
    Sidebar, // Adding Sidebar to the components
  },
  data() {
    return {
      isSidebarCollapsed: false, // Default sidebar state (expanded)
    };
  },
  methods: {
    // Toggle Sidebar collapse
    toggleSidebar() {
      this.isSidebarCollapsed = !this.isSidebarCollapsed;
    },
  },
};
</script>

<style>
body {
  font-family: 'Arial', sans-serif; /* Default font for the application */
}

html, body, #app {
  height: 100%; 
}

@media (max-width: 768px) {
  /* Mobile Responsiveness: Make the sidebar more compact */
  .sidebar {
    width: 56px; /* Sidebar width in mobile view */
  }

  .main-content {
    padding: 2rem; /* Add padding to main content in mobile view */
  }

  .flex {
    display: flex;
    flex-direction: column;
  }

  .ml-16 {
    margin-left: 4rem; /* Sidebar expanded */
  }
  .ml-4 {
    margin-left: 1rem; /* Sidebar collapsed */
  }
}
</style>
