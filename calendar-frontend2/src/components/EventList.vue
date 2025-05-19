<template>
    <div class="event-list-container p-6">
      <h2 class="text-xl font-semibold mb-4">Event List</h2>
      <table class="min-w-full bg-white shadow-md border-collapse">
        <thead>
        <tr>
            <th class="px-4 py-2 border">Event Name</th>
            <th class="px-4 py-2 border">Frequency</th>
            <th class="px-4 py-2 border">Event Date</th>
            <th class="px-4 py-2 border">Actions</th>
        </tr>
        </thead>
        <tbody>
        <tr v-for="event in events" :key="event.eventId">
            <td class="px-4 py-2 border">{{ event.eventName }}</td>
            <td class="px-4 py-2 border">{{ event.frequency }}</td>
            <td class="px-4 py-2 border">{{ event.eventDate }}</td>
            <td class="px-4 py-2 border">
            <button @click="editEvent(event.eventId)" class="text-blue-500">Edit</button>
            <button @click="deleteEvent(event.eventId)" class="text-red-500 ml-2">Delete</button>
            </td>
        </tr>
        </tbody>
      </table>
    </div>
  </template>
  
  <script>
  import axios from '../axios';
  
  export default {
    data() {
      return {
        events: [],
      };
    },
    methods: {
      // Mengambil data event dari backend
      async fetchEvents() {
        try {
          const response = await axios.get('http://localhost:5152/events');  // Pastikan URL API benar
          this.events = response.data;
        } catch (error) {
          console.error("Error fetching events:", error);
          alert("Failed to load events.");
        }
      },
      
      // Menghapus event
      async deleteEvent(eventId) {
        try {
          await axios.delete(`http://localhost:5152/events/${eventId}`);
          this.fetchEvents();  // Menyegarkan daftar event setelah penghapusan
          alert("Event deleted successfully!");
        } catch (error) {
          console.error("Error deleting event:", error);
          alert("Failed to delete event.");
        }
      },
      
      // Mengarahkan pengguna ke halaman edit event
      editEvent(eventId) {
        console.log("Redirecting to edit event with ID:", eventId);
        this.$router.push({ name: 'EventEdit', params: { eventId } }); // Navigasi ke halaman edit
      }
    },
    mounted() {
      this.fetchEvents();  // Menarik data saat komponen pertama kali dimuat
    }
  };
  </script>
  
  <style scoped>
  .event-list-container {
    max-width: 1200px;
    margin: 0 auto;
  }
  
  button {
    cursor: pointer;
    transition: color 0.3s ease;
  }
  
  button:hover {
    color: #1D4ED8; /* Tailwind blue-600 */
  }
  
  table {
    width: 100%;
    border-collapse: collapse;
  }
  
  th, td {
    padding: 10px;
    text-align: left;
    border: 1px solid #ddd;
  }
  </style>
  