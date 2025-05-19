<!-- <template>
  <v-app>
    <!-- Sidebar Component -->
    <sidebar @navigate="navigateTo" />

    <!-- Main Content Area -->
    <v-main>
      <v-container fluid>
        <v-row>
          <!-- Header -->
          <v-col cols="12" class="d-flex justify-space-between align-center">
            <v-btn @click="toggleDrawer" icon>
              <v-icon>mdi-menu</v-icon>
            </v-btn>
            <span class="headline">Calendar Management</span>
            <v-btn @click="navigateTo('event-entry')" class="ml-auto" color="primary">Go to Event Entry</v-btn>
          </v-col>
        </v-row>

        <!-- Dynamically Imported Pages -->
        <v-row>
          <v-col v-if="currentPage === 'event-entry'" cols="12">
            <event-entry-form />
          </v-col>
          <v-col v-if="currentPage === 'calendar'" cols="12">
            <calendar-view />
          </v-col>
        </v-row>
      </v-container>
    </v-main>
  </v-app>
</template>

<script>
// Import Sidebar and Pages
import Sidebar from './Sidebar.vue';
import EventEntryForm from './EventEntryForm.vue';
import CalendarView from './CalendarView.vue';

export default {
  components: {
    Sidebar,
    EventEntryForm,
    CalendarView,
  },
  data() {
    return {
      currentPage: 'event-entry', // Default page
    };
  },
  methods: {
    navigateTo(page) {
      this.currentPage = page;
    },
    toggleDrawer() {
      this.$refs.sidebar.toggleDrawer(); // Toggle the sidebar
    },
  },
};
</script>

<style scoped>
.v-btn {
  text-transform: none;
}

.headline {
  font-size: 24px;
  font-weight: bold;
}
</style> -->
