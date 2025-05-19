<template>
    <div class="entry-container">
      <h2>Monthly Event Entry</h2>
  
      <form @submit.prevent="saveEvent">
        <div>
          <label for="eventName">Event Name</label>
          <input type="text" id="eventName" v-model="event.name" required />
        </div>
  
        <div>
          <label for="date">Date (1-31)</label>
          <input type="number" id="date" v-model="event.date" min="1" max="31" required />
        </div>
  
        <div>
          <label for="startTime">Start Time</label>
          <input type="time" id="startTime" v-model="event.startTime" required />
        </div>
  
        <div>
          <label for="endTime">End Time (optional)</label>
          <input type="time" id="endTime" v-model="event.endTime" />
        </div>
  
        <div>
          <label for="location">Location / URL</label>
          <input type="text" id="location" v-model="event.location" />
        </div>
  
        <div>
          <label for="notes">Notes</label>
          <textarea id="notes" v-model="event.notes"></textarea>
        </div>
  
        <button type="submit">Save Event</button>
      </form>
    </div>
  </template>
  
  <script setup>
  import { ref } from "vue";
  import { useRouter } from "vue-router";
  
  const router = useRouter();
  
  const event = ref({
    name: "",
    date: "",
    startTime: "",
    endTime: "",
    location: "",
    notes: "",
  });
  
  const saveEvent = () => {
    console.log("Saving event", event.value);
    // Implementasi penyimpanan event
  
    router.push({ name: "calendar" });
  };
  </script>
  
  <style scoped>
  .entry-container {
    width: 50%;
    margin: 0 auto;
    padding: 20px;
    background-color: #f9f9f9;
    border-radius: 8px;
  }
  
  form {
    display: flex;
    flex-direction: column;
  }
  
  label {
    margin: 5px 0;
  }
  
  input, textarea {
    margin-bottom: 10px;
    padding: 8px;
    border: 1px solid #ddd;
    border-radius: 4px;
  }
  
  button {
    padding: 10px;
    background-color: #2196F3;
    color: white;
    border: none;
    border-radius: 4px;
    cursor: pointer;
  }
  
  button:hover {
    background-color: #1976D2;
  }
  </style>
  