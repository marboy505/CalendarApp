<template>
    <div class="weekly-event-entry">
      <h2>Create Weekly Event</h2>
      <form @submit.prevent="submitForm">
        <div>
          <label for="eventName">Event Name</label>
          <input type="text" v-model="form.eventName" id="eventName" required />
        </div>
        <div>
          <label for="days">Days of the Week</label>
          <div>
            <input type="checkbox" v-model="form.days" value="Sun" /> Sun
            <input type="checkbox" v-model="form.days" value="Mon" /> Mon
            <input type="checkbox" v-model="form.days" value="Tue" /> Tue
            <input type="checkbox" v-model="form.days" value="Wed" /> Wed
            <input type="checkbox" v-model="form.days" value="Thu" /> Thu
            <input type="checkbox" v-model="form.days" value="Fri" /> Fri
            <input type="checkbox" v-model="form.days" value="Sat" /> Sat
          </div>
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
        <button type="submit">Save Weekly Event</button>
      </form>
    </div>
  </template>
  
  <script setup>
  import { ref } from 'vue';
  import axios from '../plugins/axios';
  
  const form = ref({
    eventName: '',
    days: [],
    startTime: '',
    endTime: '',
    location: '',
    notes: ''
  });
  
  const submitForm = async () => {
    try {
      const response = await axios.post('/weekly-events', form.value);
      console.log('Event created:', response.data);
      // Show success message or redirect
    } catch (error) {
      console.error('Error creating weekly event:', error);
    }
  };
  </script>
  
  <style scoped>
  .weekly-event-entry {
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
  