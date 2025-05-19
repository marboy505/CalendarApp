<template>
    <div class="sidebar">
      <h2>Calendar Management</h2>
      <ul>
        <li>
          <router-link to="/" class="nav-link">Dashboard</router-link>
        </li>
        <li>
          <router-link to="/calendar" class="nav-link">Calendar View</router-link>
        </li>
        <li>
          <router-link to="/event-entry" class="nav-link">Add Event</router-link>
        </li>
        <li>
          <router-link to="/event-list" class="nav-link">Event List</router-link>
        </li>
      </ul>
    </div>
  </template>
  
  <script setup>
  // Tidak ada skrip khusus yang diperlukan untuk sidebar saat ini
  </script>
  
  <style scoped>
  .sidebar {
    width: 250px;
    background-color: #2c3e50;
    color: white;
    padding: 20px;
    position: fixed;
    top: 0;
    left: 0;
    height: 100%;
    box-shadow: 2px 0 5px rgba(0, 0, 0, 0.1);
  }
  
  h2 {
    text-align: center;
    margin-bottom: 20px;
  }
  
  ul {
    list-style-type: none;
    padding: 0;
  }
  
  .nav-link {
    display: block;
    padding: 12px;
    color: white;
    text-decoration: none;
    margin: 10px 0;
    border-radius: 4px;
    transition: background-color 0.3s ease;
  }
  
  .nav-link:hover {
    background-color: #34495e;
  }
  </style>
  