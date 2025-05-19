<template>
    <div class="yearly-event-entry">
      <h2>Create Yearly Event</h2>
      <form @submit.prevent="submitForm">
        <div>
          <label for="eventName">Event Name</label>
          <input type="text" v-model="form.eventName" id="eventName" required />
        </div>
        <div>
          <label for="month">Month</label>
          <input type="text" v-model="form.month" id="month" required />
        </div>
        <div>
          <label for="dayOfMonth">Day of the Month</label>
          <input type="number" v-model="form.dayOfMonth" id="dayOfMonth" min="1" max="31" required />
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
        <button type="submit">Save Yearly Event</button>
      </form>
    </div>
  </template>
  
  <script setup>
  import { ref } from 'vue';
  import axios from '../plugins/axios';
  
  const form = ref({
    eventName: '',
    month: '',
    dayOfMonth: '',
    startTime: '',
    endTime: '',
    location: '',
    notes: ''
  });
  
  const submitForm = async () => {
    try {
      const response = await axios.post('/yearly-events', form.value);
      console.log('Event created:', response.data);
      // Show success message or redirect
    } catch (error) {
      console.error('Error creating yearly event:', error);
    }
  };
  </script>
  
  <style scoped>
  .yearly-event-entry {
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
  