<template>
  <v-app>
    <v-container>
      <v-row>
        <v-col cols="12" md="4">
          <v-card>
            <v-card-title>{{ editMode ? 'Edit' : 'Add' }} Event</v-card-title>
            <v-card-text>
              <v-form @submit.prevent="editMode ? updateEvent() : submitEvent">
                <v-text-field v-model="eventName" label="Event Name" required />
                <v-select
                  v-model="frequency"
                  :items="['Weekly', 'Monthly', 'Yearly']"
                  label="Frequency"
                  required
                />
                <!-- Weekly Event Specific -->
                <v-row v-if="frequency === 'Weekly'">
                  <v-col>
                    <v-checkbox-group v-model="selectedDays" column>
                      <v-checkbox label="Sunday" value="Sun" />
                      <v-checkbox label="Monday" value="Mon" />
                      <v-checkbox label="Tuesday" value="Tue" />
                      <v-checkbox label="Wednesday" value="Wed" />
                      <v-checkbox label="Thursday" value="Thu" />
                      <v-checkbox label="Friday" value="Fri" />
                      <v-checkbox label="Saturday" value="Sat" />
                    </v-checkbox-group>
                  </v-col>
                </v-row>

                <!-- Monthly Event Specific -->
                <v-row v-if="frequency === 'Monthly'">
                  <v-col>
                    <v-text-field
                      v-model="dayOfMonth"
                      label="Day of the Month"
                      type="number"
                      min="1"
                      max="31"
                    />
                    <v-checkbox
                      v-model="lastBusinessDay"
                      label="Last Business Day of the Month"
                    />
                  </v-col>
                </v-row>

                <!-- Yearly Event Specific -->
                <v-row v-if="frequency === 'Yearly'">
                  <v-col>
                    <v-select
                      v-model="month"
                      :items="months"
                      label="Month"
                      required
                    />
                    <v-text-field
                      v-model="dayOfMonth"
                      label="Day of the Month"
                      type="number"
                      min="1"
                      max="31"
                      required
                    />
                  </v-col>
                </v-row>

                <v-textarea v-model="notes" label="Notes" />
                <v-btn type="submit" color="primary">{{ editMode ? 'Update' : 'Add' }} Event</v-btn>
              </v-form>
            </v-card-text>
          </v-card>
        </v-col>
        <v-col cols="12" md="8">
          <v-card>
            <v-card-title>Event Calendar</v-card-title>
            <v-card-text>
              <!-- Render calendar here with editable events -->
              <v-calendar :events="events" @click:event="selectEvent" />
            </v-card-text>
          </v-card>
        </v-col>
      </v-row>
    </v-container>
  </v-app>
</template>

<script>
import { ref, onMounted } from "vue";
import axios from "axios";
import { VCalendar } from "v-calendar";

export default {
  components: {
    VCalendar
  },
  data() {
    return {
      eventName: "",
      frequency: "",
      selectedDays: [],
      dayOfMonth: "",
      lastBusinessDay: false,
      month: "",
      notes: "",
      events: [],
      months: ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"],
      editMode: false,
      currentEventId: null
    };
  },
  methods: {
    async fetchEvents() {
        try {
          const response = await axios.get("http://localhost:5113/events");
          this.events = response.data.map(event => {
            // Calculate the event's start date and end date based on frequency
            let startDate = new Date();
            let endDate = new Date();
            
            if (event.Frequency === "Weekly") {
              // Weekly events, e.g., every Monday
              startDate.setDate(startDate.getDate() + (7 - startDate.getDay() + event.DayOfWeek) % 7);
              endDate.setDate(startDate.getDate() + 1); // End date 1 day after start
            } else if (event.Frequency === "Monthly") {
              // Monthly events, e.g., 15th of each month
              startDate.setDate(event.DayOfMonth);
              endDate.setDate(startDate.getDate() + 1);
            } else if (event.Frequency === "Yearly") {
              // Yearly events, e.g., 15 June
              startDate.setMonth(new Date(event.Month + " 1").getMonth());
              startDate.setDate(event.DayOfMonth);
              endDate.setDate(startDate.getDate() + 1);
            }

            return {
              title: event.EventName,
              start: startDate.toISOString().split("T")[0],  // YYYY-MM-DD format
              end: endDate.toISOString().split("T")[0],      // YYYY-MM-DD format
              frequency: event.Frequency,
              notes: event.Notes
            };
          });
        } catch (error) {
          console.error("Error fetching events:", error);
        }
      }
    async submitEvent() {
      const newEvent = {
        EventName: this.eventName,
        Frequency: this.frequency,
        Notes: this.notes
      };

      if (this.frequency === "Weekly") {
        newEvent.DaysOfWeek = this.selectedDays.join(",");
      } else if (this.frequency === "Monthly") {
        newEvent.DayOfMonth = this.dayOfMonth;
        newEvent.LastBusinessDay = this.lastBusinessDay;
      } else if (this.frequency === "Yearly") {
        newEvent.Month = this.month;
        newEvent.DayOfMonth = this.dayOfMonth;
      }

      try {
        await axios.post("http://localhost:5113/events", newEvent);
        this.fetchEvents(); // Reload events after submitting
        this.clearForm(); // Clear form fields
      } catch (error) {
        console.error("Error submitting event:", error);
      }
    },
    async updateEvent() {
      const updatedEvent = {
        EventName: this.eventName,
        Frequency: this.frequency,
        Notes: this.notes
      };

      if (this.frequency === "Weekly") {
        updatedEvent.DaysOfWeek = this.selectedDays.join(",");
      } else if (this.frequency === "Monthly") {
        updatedEvent.DayOfMonth = this.dayOfMonth;
        updatedEvent.LastBusinessDay = this.lastBusinessDay;
      } else if (this.frequency === "Yearly") {
        updatedEvent.Month = this.month;
        updatedEvent.DayOfMonth = this.dayOfMonth;
      }

      try {
        await axios.put(`http://localhost:5113/events/${this.currentEventId}`, updatedEvent);
        this.fetchEvents(); // Reload events after update
        this.clearForm(); // Clear form fields
        this.editMode = false; // Switch to add mode
      } catch (error) {
        console.error("Error updating event:", error);
      }
    },
    filterEvents() {
      if (this.selectedFrequency === 'All') {
        this.fetchEvents(); // Get all events
      } else {
        // Filter the events by frequency
        this.events = this.events.filter(event => event.Frequency === this.selectedFrequency);
      }
    },
    clearForm() {
      this.eventName = "";
      this.frequency = "";
      this.selectedDays = [];
      this.dayOfMonth = "";
      this.lastBusinessDay = false;
      this.month = "";
      this.notes = "";
    },
    selectEvent(event) {
      this.currentEventId = event.id;
      this.eventName = event.EventName;
      this.frequency = event.Frequency;
      this.notes = event.Notes;
      
      if (event.Frequency === "Weekly") {
        this.selectedDays = event.DaysOfWeek.split(",");
      } else if (event.Frequency === "Monthly") {
        this.dayOfMonth = event.DayOfMonth;
        this.lastBusinessDay = event.LastBusinessDay;
      } else if (event.Frequency === "Yearly") {
        this.month = event.Month;
        this.dayOfMonth = event.DayOfMonth;
      }

      this.editMode = true;
    }
  },
  onMounted() {
    this.fetchEvents(); // Load events when the component is mounted
  }
};
</script>
