<template>
    <div class="space-y-4">
      <h2 class="text-3xl font-bold text-center">Event Entry</h2>
      <form @submit.prevent="submitForm">
        <!-- Event Name -->
        <div class="mb-4">
          <label for="eventName" class="block">Event Name</label>
          <input
            v-model="eventName"
            type="text"
            id="eventName"
            class="w-full p-2 border border-gray-300 rounded"
            placeholder="Enter Event Name"
          />
        </div>
  
        <!-- Date Picker (Day of the Month) -->
        <div class="mb-4">
          <label for="eventDate" class="block">Event Date</label>
          <v-calendar
            v-model="selectedDate"
            :attributes="calendarAttributes"
            is-inline
            locale="en"
            color="primary"
            class="w-full"
          />
        </div>
  
        <!-- Frequency Selection -->
        <div class="mb-4">
          <label for="frequency" class="block">Event Frequency</label>
          <select
            v-model="frequency"
            id="frequency"
            class="w-full p-2 border border-gray-300 rounded"
          >
            <option value="Monthly">Monthly</option>
            <option value="Weekly">Weekly</option>
            <option value="Yearly">Yearly</option>
          </select>
        </div>
  
        <!-- Start Time -->
        <div class="mb-4">
          <label for="startTime" class="block">Start Time</label>
          <input
            v-model="startTime"
            type="time"
            id="startTime"
            class="w-full p-2 border border-gray-300 rounded"
          />
        </div>
  
        <!-- Location -->
        <div class="mb-4">
          <label for="location" class="block">Location</label>
          <input
            v-model="location"
            type="text"
            id="location"
            class="w-full p-2 border border-gray-300 rounded"
            placeholder="Enter Event Location"
          />
        </div>
  
        <!-- Notes -->
        <div class="mb-4">
          <label for="notes" class="block">Notes</label>
          <textarea
            v-model="notes"
            id="notes"
            rows="4"
            class="w-full p-2 border border-gray-300 rounded"
            placeholder="Enter event notes"
          ></textarea>
        </div>
  
        <button type="submit" class="bg-blue-500 text-white p-2 rounded">Submit</button>
      </form>
    </div>
  </template>
  
  <script>
  import VCalendar from 'v-calendar';
  
  export default {
    components: {
      VCalendar,
    },
    data() {
      return {
        eventName: '',
        selectedDate: null, // To store selected date
        frequency: 'Monthly', // Default to monthly
        startTime: '',
        location: '',
        notes: '',
        calendarAttributes: [], // To customize calendar attributes
      };
    },
    methods: {
      async submitForm() {
        // If no date is selected, show alert
        if (!this.selectedDate) {
          alert('Please select a date for the event');
          return;
        }
  
        const eventData = {
          eventName: this.eventName,
          frequency: this.frequency,
          eventDate: this.selectedDate,
          startTime: this.startTime,
          location: this.location,
          notes: this.notes,
        };
  
        try {
          const response = await fetch('http://localhost:5152/events', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(eventData),
          });
          if (response.ok) {
            alert('Event added successfully!');
            this.resetForm();
          } else {
            alert('Failed to add event.');
          }
        } catch (error) {
          console.error('Error submitting form:', error);
        }
      },
      resetForm() {
        this.eventName = '';
        this.selectedDate = null;
        this.frequency = 'Monthly';
        this.startTime = '';
        this.location = '';
        this.notes = '';
      },
    },
  };
  </script>
  
  <style scoped>
  form {
    max-width: 600px;
    margin: 0 auto;
  }
  
  input,
  textarea,
  select {
    width: 100%;
    padding: 8px;
    margin-bottom: 10px;
    border: 1px solid #ccc;
    border-radius: 4px;
  }
  
  button {
    background-color: #3498db;
    color: white;
    padding: 10px 15px;
    border-radius: 4px;
    cursor: pointer;
  }
  
  button:hover {
    background-color: #2980b9;
  }
  </style>
  