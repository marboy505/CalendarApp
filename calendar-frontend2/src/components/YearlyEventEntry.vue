<template>
    <div class="space-y-4">
      <h2 class="text-3xl font-bold text-center">Yearly Event Entry</h2>
      <form @submit.prevent="submitForm">
        <div class="mb-4">
          <label for="eventName" class="block">Event Name</label>
          <input
            v-model="eventName"
            type="text"
            id="eventName"
            class="w-full p-2 border border-gray-300 rounded"
          />
        </div>
        <div class="mb-4">
          <label for="month" class="block">Month</label>
          <input
            v-model="month"
            type="text"
            id="month"
            class="w-full p-2 border border-gray-300 rounded"
          />
        </div>
        <div class="mb-4">
          <label for="dayOfMonth" class="block">Day of the Month</label>
          <input
            v-model="dayOfMonth"
            type="number"
            id="dayOfMonth"
            class="w-full p-2 border border-gray-300 rounded"
          />
        </div>
        <div class="mb-4">
          <label for="startTime" class="block">Start Time</label>
          <input
            v-model="startTime"
            type="time"
            id="startTime"
            class="w-full p-2 border border-gray-300 rounded"
          />
        </div>
        <div class="mb-4">
          <label for="location" class="block">Location</label>
          <input
            v-model="location"
            type="text"
            id="location"
            class="w-full p-2 border border-gray-300 rounded"
          />
        </div>
        <div class="mb-4">
          <label for="notes" class="block">Notes</label>
          <textarea
            v-model="notes"
            id="notes"
            rows="4"
            class="w-full p-2 border border-gray-300 rounded"
          ></textarea>
        </div>
        <button type="submit" class="bg-blue-500 text-white p-2 rounded">Submit</button>
      </form>
    </div>
  </template>
  
  <script>
  export default {
    data() {
      return {
        eventName: '',
        month: '',
        dayOfMonth: '',
        startTime: '',
        location: '',
        notes: ''
      }
    },
    methods: {
      async submitForm() {
        const eventData = {
          eventName: this.eventName,
          frequency: 'Yearly',
          month: this.month,
          dayOfMonth: this.dayOfMonth,
          startTime: this.startTime,
          location: this.location,
          notes: this.notes
        }
  
        try {
          const response = await fetch('http://localhost:5152/events', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(eventData)
          })
          if (response.ok) {
            alert('Event added successfully!')
          } else {
            alert('Failed to add event.')
          }
        } catch (error) {
          console.error('Error submitting form:', error)
        }
      }
    }
  }
  </script>
  
  <style scoped>
  form {
    max-width: 600px;
    margin: 0 auto;
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
  