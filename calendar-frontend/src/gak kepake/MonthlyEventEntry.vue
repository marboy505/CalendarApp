<template>
    <div class="monthly-event-entry">
      <h2>Create Monthly Event</h2>
      <form @submit.prevent="submitForm">
        <div>
          <label for="eventName">Event Name</label>
          <input type="text" v-model="form.eventName" id="eventName" required />
        </div>
        <div>
          <label for="eventDate">Event Date</label>
          <FullCalendar
            :events="calendarEvents"
            :dateClick="handleDateClick"
            :initialView="'dayGridMonth'"
          />
        </div>
        <div>
          <label for="startTime">Start Time</label>
          <input type="time" v-model="form.startTime" id="startTime" required />
        </div>
        <div>
          <label for="endTime">End Time</label>
          <input type="time" v-model="form.endTime" id="endTime" />
        </div>
        <div>
          <label for="location">Location</label>
          <input type="text" v-model="form.location" id="location" />
        </div>
        <div>
          <label for="notes">Notes</label>
          <textarea v-model="form.notes" id="notes"></textarea>
        </div>
        <button type="submit">Save Monthly Event</button>
      </form>
    </div>
  </template>
  
  <script setup>
  import { ref } from 'vue';
  import axios from '../plugins/axios';
  import FullCalendar from '@fullcalendar/vue3'; // Import FullCalendar
  
  const form = ref({
    eventName: '',
    eventDate: '', // Store selected date from calendar
    startTime: '',
    endTime: '',
    location: '',
    notes: ''
  });
  
  // Store calendar events
  const calendarEvents = ref([]);
  
  // Handle date selection from calendar
  const handleDateClick = (info) => {
    form.value.eventDate = info.dateStr; // Capture selected date
    console.log('Selected Date:', form.value.eventDate);
  };
  
  // Submit event form to the backend
  const submitForm = async () => {
    try {
      // Post the event data to the backend
      const response = await axios.post('/monthly-events', form.value);
      console.log('Event created:', response.data);
      // You can add a success message or redirect here
    } catch (error) {
      console.error('Error creating monthly event:', error);
    }
  };
  </script>
  
  <style scoped>
  .monthly-event-entry {
    padding: 20px;
  }
  
  form {
    display: flex;
    flex-direction: column;
  }
  
  div {
    margin-bottom: 15px;
  }
  
  label {
    margin-bottom: 5px;
  }
  
  input, textarea {
    padding: 10px;
    width: 100%;
  }
  
  button {
    padding: 10px;
    background-color: #42b883;
    color: white;
    border: none;
    cursor: pointer;
  }
  </style>
  