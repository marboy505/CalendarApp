<template>
  <v-app>
    <!-- Sidebar -->
    <v-navigation-drawer
      v-model="drawer"
      app
      temporary
      :clipped="$vuetify.breakpoint.smAndDown"
      class="sidebar"
    >
      <v-list dense>
        <v-list-item link @click="goToPage('iot-management')">
          <v-list-item-icon>
            <v-icon>mdi-devices</v-icon>
          </v-list-item-icon>
          <v-list-item-title>IoT Management</v-list-item-title>
        </v-list-item>
        <v-list-item link @click="goToPage('calendar')">
          <v-list-item-icon>
            <v-icon>mdi-calendar-multiple-check</v-icon>
          </v-list-item-icon>
          <v-list-item-title>Calendar</v-list-item-title>
        </v-list-item>
        <v-list-item link @click="goToPage('entry-form')">
          <v-list-item-icon>
            <v-icon>mdi-plus-circle</v-icon>
          </v-list-item-icon>
          <v-list-item-title>Event Entry</v-list-item-title>
        </v-list-item>
      </v-list>
    </v-navigation-drawer>

    <!-- Header -->
    <v-app-bar app clipped-left>
      <v-toolbar-title>IoT Management</v-toolbar-title>
      <v-spacer></v-spacer>
      <v-btn @click="drawer = !drawer" icon>
        <v-icon>mdi-menu</v-icon>
      </v-btn>
    </v-app-bar>

    <!-- Main Content -->
    <v-main>
      <v-container>
        <router-view></router-view>
      </v-container>
    </v-main>
  </v-app>
</template>

<script setup>
import { ref } from 'vue';

const drawer = ref(false);  // Mengontrol status sidebar

const goToPage = (page) => {
  this.$router.push({ name: page });
};
</script>
