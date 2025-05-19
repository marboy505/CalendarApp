<template>
  <div>
    <h2>Event List</h2>
    <table>
      <thead>
        <tr>
          <th>ID</th>
          <th>Event Name</th>
          <th>Frequency</th>
          <th>Notes</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="event in events" :key="event.eventId">
          <td>{{ event.eventId }}</td>
          <td>{{ event.eventName }}</td>
          <td>{{ event.frequency }}</td>
          <td>{{ event.notes }}</td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script>
import axios from 'axios';

export default {
  data() {
    return {
      events: []
    };
  },
  mounted() {
    this.fetchEvents();
  },
  methods: {
    fetchEvents() {
      axios.get('http://localhost:5152/events')
        .then(response => {
          this.events = response.data;
        })
        .catch(error => {
          console.error("Error fetching events:", error);
        });
    }
  }
};
</script>


<style scoped>
.event-list-page {
  margin: 20px;
}

button {
  padding: 8px 16px;
  margin-top: 10px;
}
</style>
