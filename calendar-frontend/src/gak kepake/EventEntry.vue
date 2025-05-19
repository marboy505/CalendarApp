<template>
  <div class="event-entry-page">
    <h1>Add Event</h1>
    <div>
      <button @click="setEventType('weekly')">Weekly Event</button>
      <button @click="setEventType('monthly')">Monthly Event</button>
      <button @click="setEventType('yearly')">Yearly Event</button>
    </div>

    <!-- Kondisi tampilan berdasarkan jenis event -->
    <div v-if="eventType === 'weekly'">
      <WeeklyEventEntry @submit-event="submitEvent" />
    </div>
    <div v-if="eventType === 'monthly'">
      <MonthlyEventEntry @submit-event="submitEvent" />
    </div>
    <div v-if="eventType === 'yearly'">
      <YearlyEventEntry @submit-event="submitEvent" />
    </div>
  </div>
</template>

<script>
import { ref } from "vue";
import axios from "axios";
import WeeklyEventEntry from "../components/WeeklyEventEntry.vue";
import MonthlyEventEntry from "../components/MonthlyEventEntry.vue";
import YearlyEventEntry from "../components/YearlyEventEntry.vue";

export default {
  name: "EventEntry",
  components: {
    WeeklyEventEntry,
    MonthlyEventEntry,
    YearlyEventEntry
  },
  setup() {
    const eventType = ref(null);

    const setEventType = (type) => {
      eventType.value = type;
    };

    // Fungsi untuk mengirim event ke backend
    const submitEvent = async (eventData) => {
      try {
        let response;
        // Menentukan URL endpoint berdasarkan tipe event
        if (eventType.value === "weekly") {
          response = await axios.post("http://localhost:5152/weekly-events", eventData); // Ganti dengan URL API backend Anda
        } else if (eventType.value === "monthly") {
          response = await axios.post("http://localhost:5152/monthly-events", eventData); // Ganti dengan URL API backend Anda
        } else if (eventType.value === "yearly") {
          response = await axios.post("http://localhost:5152/yearly-events", eventData); // Ganti dengan URL API backend Anda
        }

        console.log("Event added successfully", response.data);
        // Lakukan tindakan setelah sukses, seperti mengarahkan pengguna ke halaman kalender atau menampilkan pesan sukses.
      } catch (error) {
        console.error("Error adding event:", error);
      }
    };

    return {
      eventType,
      setEventType,
      submitEvent
    };
  }
};
</script>

<style scoped>
.event-entry-page {
  margin: 20px;
}

button {
  margin: 10px;
  padding: 10px;
  background-color: #4CAF50;
  color: white;
  border: none;
  cursor: pointer;
}

button:hover {
  background-color: #45a049;
}
</style>
