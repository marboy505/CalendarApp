<template>
    <div class="event-card bg-white shadow-md p-4 mb-4 rounded-lg">
      <h3 class="font-semibold text-lg">{{ event.eventName }}</h3>
      <p><strong>Frequency:</strong> {{ event.frequency }}</p>
      <p><strong>Notes:</strong> {{ event.notes }}</p>
      <button @click="goToEventPage(event.eventId)" class="mt-2 text-blue-500">View Details</button>
    </div>
  </template>
  
  <script>
  export default {
    props: {
      event: Object
    },
    methods: {
      goToEventPage(eventId) {
        // Navigate to the event detail page
        this.$router.push(`/events/${eventId}`);
      }
    }
  };
  </script>
  
  <style scoped>
  .event-card {
    cursor: pointer;
  }
  
  .event-card:hover {
    transform: scale(1.05);
    transition: transform 0.2s;
  }
  
  button {
    cursor: pointer;
  }
  </style>
  