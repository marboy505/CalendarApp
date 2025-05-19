<template>
    <div class="event-detail">
      <h2 class="text-3xl font-bold" v-if="event.eventName">{{ event.eventName }}</h2>
      <p class="text-lg" v-if="event.notes">{{ event.notes }}</p>
      <p v-if="event.frequency"><strong>Frequency:</strong> {{ event.frequency }}</p>
      <p v-if="event.location"><strong>Location:</strong> {{ event.location }}</p>
      <p v-if="event.startTime"><strong>Start Time:</strong> {{ formatDateTime(event.startTime) }}</p>
      
      <div v-if="!event.eventName">
        <p class="text-red-500">Event not found. Please check the event ID.</p>
      </div>
  
      <button 
        v-if="event.eventName" 
        @click="editEvent" 
        class="mt-4 bg-blue-500 text-white p-2 rounded"
      >
        Edit Event
      </button>
    </div>
  </template>
  
  <script>
  export default {
    data() {
      return {
        event: {}
      }
    },
    async created() {
      const eventId = this.$route.params.eventId;
      try {
        const response = await fetch(`http://localhost:5152/events/${eventId}`);
        if (!response.ok) {
          throw new Error('Event not found');
        }
        this.event = await response.json();
      } catch (error) {
        console.error('Error fetching event:', error);
        this.event = { eventName: '', notes: '' }; // Set empty data if not found
      }
    },
    methods: {
      editEvent() {
        this.$router.push({ name: 'EventEdit', params: { eventId: this.event.eventId } });
      },
      formatDateTime(dateTime) {
        const date = new Date(dateTime);
        return `${date.toLocaleDateString()} ${date.toLocaleTimeString()}`;
      }
    }
  }
  </script>
  
  <style scoped>
  .event-detail {
    padding: 20px;
  }
  
  button {
    cursor: pointer;
  }
  
  button:hover {
    background-color: #2b6cb0;
    transition: background-color 0.3s;
  }
  </style>
  