<template>
  <v-container>
    <v-row>
      <v-col cols="12">
        <v-card>
          <v-card-title>
            <span class="headline">EVENTS & FREQUENCY LIST</span>
          </v-card-title>
          <v-data-table
            :headers="headers"
            :items="events"
            item-key="EventId"
            class="elevation-1"
          >
            <template v-slot:item="props">
              <tr @click="goToEvent(props.item.EventId)">
                <td>{{ props.item.EventId }}</td>
                <td>{{ props.item.EventName }}</td>
                <td>{{ props.item.Frequency }}</td>
                <td>{{ props.item.Notes }}</td>
              </tr>
            </template>
          </v-data-table>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import axios from 'axios';

const headers = [
  { text: 'ID', align: 'start', key: 'EventId', sortable: true },
  { text: 'Event Name', align: 'start', key: 'EventName', sortable: true },
  { text: 'Frequency', align: 'start', key: 'Frequency', sortable: true },
  { text: 'Notes', align: 'start', key: 'Notes' }
];

const events = ref([]);

onMounted(async () => {
  try {
    const response = await axios.get('http://localhost:5113/events');
    events.value = response.data;
  } catch (error) {
    console.error('Error fetching events:', error);
  }
});

const goToEvent = (eventId) => {
  // Navigasi ke halaman detail event jika diperlukan
  console.log(`Navigate to event details for ID: ${eventId}`);
};
</script>
