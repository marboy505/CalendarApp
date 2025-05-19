<template>
    <v-container>
      <v-row>
        <v-col cols="12">
          <v-card>
            <v-card-title>
              <span class="headline">Calendar</span>
            </v-card-title>
            <v-calendar v-model="selectedDate" :events="eventsInCalendar" />
            <v-divider />
            <div v-if="selectedEvent">
              <v-row>
                <v-col cols="12">
                  <v-card>
                    <v-card-title>{{ selectedEvent.EventName }}</v-card-title>
                    <v-card-subtitle>{{ selectedEvent.Notes }}</v-card-subtitle>
                  </v-card>
                </v-col>
              </v-row>
            </div>
            <div v-else>
              <p>No events today.</p>
            </div>
          </v-card>
        </v-col>
      </v-row>
    </v-container>
  </template>
  
  <script setup>
  import { ref, onMounted } from 'vue';
  import axios from 'axios';
  
  const selectedDate = ref(null);
  const selectedEvent = ref(null);
  const eventsInCalendar = ref([]);
  
  onMounted(async () => {
    try {
      const response = await axios.get('http://localhost:5113/events');
      eventsInCalendar.value = response.data.map(event => ({
        date: event.EventDate,
        name: event.EventName,
        notes: event.Notes,
      }));
    } catch (error) {
      console.error('Error fetching events:', error);
    }
  });
  
  const selectDate = (date) => {
    const event = eventsInCalendar.value.find(event => event.date === date);
    selectedEvent.value = event || null;
  };
  </script>
  