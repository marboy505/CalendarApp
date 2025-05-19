<template>
  <v-container>
    <v-row>
      <v-col cols="12">
        <v-card>
          <v-card-title>
            <span class="headline">EVENT ENTRY FORMS</span>
          </v-card-title>

          <v-radio-group v-model="selectedFrequency" column>
            <v-radio label="Weekly" value="weekly" />
            <v-radio label="Monthly" value="monthly" />
            <v-radio label="Yearly" value="yearly" />
          </v-radio-group>

          <!-- Weekly Event Form -->
          <div v-if="selectedFrequency === 'weekly'">
            <v-form>
              <v-text-field label="Event Name" v-model="weeklyEvent.name" />
              <v-checkbox-group v-model="weeklyEvent.days">
                <v-checkbox label="Sun" value="Sun" />
                <v-checkbox label="Mon" value="Mon" />
                <v-checkbox label="Tue" value="Tue" />
                <v-checkbox label="Wed" value="Wed" />
                <v-checkbox label="Thu" value="Thu" />
                <v-checkbox label="Fri" value="Fri" />
                <v-checkbox label="Sat" value="Sat" />
              </v-checkbox-group>
              <v-time-picker v-model="weeklyEvent.startTime" label="Start Time" />
              <v-time-picker v-model="weeklyEvent.endTime" label="End Time" />
              <v-text-field label="Location / URL" v-model="weeklyEvent.location" />
              <v-textarea label="Notes" v-model="weeklyEvent.notes" />
              <v-btn @click="submitWeeklyEvent">Submit Weekly Event</v-btn>
            </v-form>
          </div>

          <!-- Monthly Event Form -->
          <div v-if="selectedFrequency === 'monthly'">
            <v-form>
              <v-text-field label="Event Name" v-model="monthlyEvent.name" />
              <v-text-field label="Date (1-31)" v-model="monthlyEvent.date" />
              <v-checkbox label="Last Business Day" v-model="monthlyEvent.lastBusinessDay" />
              <v-time-picker v-model="monthlyEvent.startTime" label="Start Time" />
              <v-time-picker v-model="monthlyEvent.endTime" label="End Time" />
              <v-text-field label="Location / URL" v-model="monthlyEvent.location" />
              <v-textarea label="Notes" v-model="monthlyEvent.notes" />
              <v-btn @click="submitMonthlyEvent">Submit Monthly Event</v-btn>
            </v-form>
          </div>

          <!-- Yearly Event Form -->
          <div v-if="selectedFrequency === 'yearly'">
            <v-form>
              <v-text-field label="Event Name" v-model="yearlyEvent.name" />
              <v-select label="Month" :items="months" v-model="yearlyEvent.month" />
              <v-text-field label="Day (1-31)" v-model="yearlyEvent.day" />
              <v-time-picker v-model="yearlyEvent.startTime" label="Start Time" />
              <v-time-picker v-model="yearlyEvent.endTime" label="End Time" />
              <v-text-field label="Location / URL" v-model="yearlyEvent.location" />
              <v-textarea label="Notes" v-model="yearlyEvent.notes" />
              <v-btn @click="submitYearlyEvent">Submit Yearly Event</v-btn>
            </v-form>
          </div>

        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>

<script setup>
import { ref } from 'vue';
import axios from 'axios';

const selectedFrequency = ref('');
const weeklyEvent = ref({
  name: '',
  days: [],
  startTime: '',
  endTime: '',
  location: '',
  notes: '',
});
const monthlyEvent = ref({
  name: '',
  date: '',
  lastBusinessDay: false,
  startTime: '',
  endTime: '',
  location: '',
  notes: '',
});
const yearlyEvent = ref({
  name: '',
  month: '',
  day: '',
  startTime: '',
  endTime: '',
  location: '',
  notes: '',
});

const months = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];

const submitWeeklyEvent = () => {
  console.log(weeklyEvent.value);
  // API call to save weekly event
};

const submitMonthlyEvent = () => {
  console.log(monthlyEvent.value);
  // API call to save monthly event
};

const submitYearlyEvent = () => {
  console.log(yearlyEvent.value);
  // API call to save yearly event
};
</script>
