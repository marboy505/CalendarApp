<template>
    <div class="edit-event">
      <h2 class="text-3xl font-bold">Edit Event</h2>
      <form @submit.prevent="submitForm">
        <div class="mb-4">
          <label for="eventName">Event Name</label>
          <input v-model="event.eventName" type="text" id="eventName" class="w-full p-2 border" required />
        </div>
        <div class="mb-4">
          <label for="notes">Notes</label>
          <textarea v-model="event.notes" id="notes" class="w-full p-2 border" required></textarea>
        </div>
        <div class="mb-4">
          <label for="startTime">Start Time</label>
          <input v-model="event.startTime" type="time" id="startTime" class="w-full p-2 border" required />
        </div>
        <button type="submit" class="bg-blue-500 text-white p-2 rounded">Save Changes</button>
      </form>
    </div>
  </template>
  
  <script>
  export default {
    data() {
      return {
        event: {
          eventId: null,  // To store event ID
          eventName: '',
          notes: '',
          startTime: ''
        }
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
        alert('Event not found. Please check the event ID.');
        this.$router.push({ name: 'EventList' });
      }
    },
    methods: {
      async submitForm() {
        try {
          const response = await fetch(`http://localhost:5152/events/${this.event.eventId}`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(this.event)
          });
          if (response.ok) {
            alert('Event updated successfully!');
            this.$router.push({ name: 'EventList' });
          } else {
            alert('Failed to update event.');
          }
        } catch (error) {
          console.error('Error updating event:', error);
          alert('Failed to update event. Please try again later.');
        }
      }
    }
  }
  </script>
  
  <style scoped>
  .edit-event {
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
  