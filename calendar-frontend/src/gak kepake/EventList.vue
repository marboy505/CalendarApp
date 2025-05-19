<template>
    <div class="event-list">
      <h2>Event List</h2>
      <table>
        <thead>
          <tr>
            <th>ID</th>
            <th>Event Name</th>
            <th>Frequency</th>
            <th>Notes</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="event in events" :key="event.EventId">
            <td>{{ event.EventId }}</td>
            <td>{{ event.EventName }}</td>
            <td>{{ event.Frequency }}</td>
            <td>{{ event.Notes }}</td>
            <td>
              <button @click="deleteEvent(event.EventId)">Delete</button>
            </td>
          </tr>
        </tbody>
      </table>
      <button @click="goToAddEventPage">Add Event</button>
    </div>
  </template>
  
  <script setup>
  import { ref, onMounted } from 'vue';
  import axios from 'axios';
  import { useRouter } from 'vue-router';
  
  const events = ref([]);
  const router = useRouter();
  
  // Fetch events from the backend
  const fetchEvents = async () => {
    try {
      const response = await axios.get('http://localhost:5000/events'); // URL Backend Anda
      events.value = response.data;
    } catch (error) {
      console.error("There was an error fetching the events:", error);
    }
  };
  
  // Delete event
  const deleteEvent = async (eventId) => {
    try {
      await axios.delete(`http://localhost:5000/events/${eventId}`);
      fetchEvents(); // Re-fetch events after deletion
    } catch (error) {
      console.error("Error deleting event:", error);
    }
  };
  
  // Navigate to the add event page
  const goToAddEventPage = () => {
    router.push('/event-entry');
  };
  
  // Fetch the events when the component is mounted
  onMounted(fetchEvents);
  </script>
  
  <style scoped>
  .event-list {
    padding: 20px;
  }
  
  h2 {
    margin-bottom: 20px;
  }
  
  table {
    width: 100%;
    border-collapse: collapse;
    margin-bottom: 20px;
  }
  
  table th, table td {
    padding: 8px;
    border: 1px solid #ddd;
    text-align: left;
  }
  
  button {
    padding: 10px 15px;
    background-color: #007bff;
    color: white;
    border: none;
    cursor: pointer;
  }
  
  button:hover {
    background-color: #0056b3;
  }
  </style>
  