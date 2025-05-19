<template>
    <div class="space-y-6">
      <!-- ===== Header ===== -->
      <div class="flex items-center mb-4">
        <router-link
          to="/event-list"
          class="px-4 py-2 text-blue-500 border rounded hover:bg-gray-700 mr-auto"
        >
          ← Back to Event List
        </router-link>
        <h2 class="text-3xl font-bold text-center flex-1">Event Entry</h2>
      </div>
  
      <!-- ===== Form ===== -->
      <v-form @submit.prevent="submitForm" class="space-y-4">
        <!-- Event Name -->
        <div>
          <label class="block">Event Name</label>
          <v-text-field
            v-model="eventName"
            density="comfortable"
            :error-messages="errors.eventName"
          />
        </div>
  
        <!-- Date-picker (Monthly & Yearly) -->
        <div v-if="frequency !== 'Weekly'">
          <label class="block">Event Date</label>
          <v-date-picker
            v-model="selectedDate"
            is-inline
            locale="id"
            :attributes="dateHighlight"
            @dayclick="d => (selectedDate = d.date)"
          />
          <span v-if="errors.selectedDate" class="text-red-500 text-sm">
            {{ errors.selectedDate }}
          </span>
        </div>
  
        <!-- Day-of-Week selector (Weekly) -->
        <div v-else>
          <label class="block">Day of Week</label>
          <v-select
            v-model="dayOfWeek"
            :items="weekItems"
            item-title="label"
            item-value="value"
            density="comfortable"
            :error-messages="errors.dayOfWeek"
          />
        </div>
  
        <!-- Frequency -->
        <div>
          <label class="block">Event Frequency</label>
          <v-select
            v-model="frequency"
            :items="['Monthly', 'Weekly', 'Yearly']"
            density="comfortable"
            :error-messages="errors.frequency"
          />
        </div>
  
        <!-- Notes -->
        <div>
          <label class="block">Notes</label>
          <v-textarea v-model="notes" density="comfortable" />
        </div>
  
        <!-- Submit -->
        <v-btn
          type="submit"
          color="primary"
          :loading="isSubmitting"
          :disabled="isSubmitting"
          class="w-full"
        >
          Submit
        </v-btn>
  
        <!-- Status -->
        <span
          v-if="submissionStatus"
          :class="submissionStatus.success ? 'text-green-500' : 'text-red-500'"
        >
          {{ submissionStatus.message }}
        </span>
      </v-form>
    </div>
  </template>
  
  <script setup>
  import { ref, reactive, computed, watch } from 'vue'
  import { useRouter } from 'vue-router'
  
  /* ---------- reactive state ---------- */
  const eventName    = ref('')
  const selectedDate = ref(null)          // ISO string | null
  const frequency    = ref('Monthly')
  const dayOfWeek    = ref(null)          // 'Mon', ...
  const notes        = ref('')
  const isSubmitting = ref(false)
  const submissionStatus = ref(null)
  const errors       = reactive({})
  
  /* ---------- helpers ---------- */
  const weekItems = [
    { label: 'Senin',  value: 'Mon' },
    { label: 'Selasa', value: 'Tue' },
    { label: 'Rabu',   value: 'Wed' },
    { label: 'Kamis',  value: 'Thu' },
    { label: 'Jumat',  value: 'Fri' },
    { label: 'Sabtu',  value: 'Sat' },
    { label: 'Minggu', value: 'Sun' },
  ]
  
  const dateHighlight = computed(() =>
    selectedDate.value
      ? [{ dates: selectedDate.value, highlight: { class: 'bg-teal-400' } }]
      : []
  )
  
  const monthShort   = d => d.toLocaleString('en-US', { month: 'short' }) // "May"
  const router       = useRouter()
  
  /* ---------- reset on frequency change ---------- */
  watch(frequency, f => {
    if (f === 'Weekly')  selectedDate.value = null
    else                 dayOfWeek.value   = null
  })
  
  /* ---------- submit ---------- */
  async function submitForm () {
    /* --- validation --- */
    errors.eventName    = !eventName.value.trim() ? 'Event Name wajib diisi' : ''
    errors.frequency    = !frequency.value ? 'Pilih frekuensi' : ''
    errors.selectedDate = ''
    errors.dayOfWeek    = ''
  
    if (frequency.value === 'Weekly') {
      errors.dayOfWeek = !dayOfWeek.value ? 'Pilih hari' : ''
    } else {
      errors.selectedDate = !selectedDate.value ? 'Pilih tanggal' : ''
    }
  
    if (Object.values(errors).some(Boolean)) return
  
    isSubmitting.value = true
    submissionStatus.value = null
  
    /* --- primitive copies to avoid circular JSON --- */
    const nameText   = eventName.value.trim()
    const noteText   = notes.value.trim()
    const freqText   = frequency.value
    const dateISO    = selectedDate.value         // may be null
    const dowText    = dayOfWeek.value            // may be null
  
    try {
      /* 1. Event master */
      const res = await fetch('http://localhost:5152/events', {
        method : 'POST',
        headers: { 'Content-Type':'application/json' },
        body   : JSON.stringify({
          eventName : nameText,
          eventDate : dateISO,
          frequency : freqText,
          notes     : noteText
        })
      })
      if (!res.ok) throw new Error(await res.text())
      const { eventId } = await res.json()
  
      /* 2. Detail schedule */
      const base = { method:'POST', headers:{'Content-Type':'application/json'} }
  
      if (freqText === 'Weekly') {
        await fetch('http://localhost:5152/weekly-events', {
          ...base,
          body: JSON.stringify({ eventId, dayOfWeek: dowText, notes: noteText })
        })
      } else {
        const dObj = new Date(dateISO)
        const dom  = dObj.getDate()
        const mon  = monthShort(dObj)
  
        if (freqText === 'Monthly') {
          await fetch('http://localhost:5152/monthly-events', {
            ...base,
            body: JSON.stringify({ eventId, dayOfMonth: dom, month: mon, notes: noteText })
          })
        } else { // Yearly
          await fetch('http://localhost:5152/yearly-events', {
            ...base,
            body: JSON.stringify({ eventId, dayOfMonth: dom, month: mon, notes: noteText })
          })
        }
      }
  
      submissionStatus.value = { success:true, message:'Berhasil disimpan!' }
      resetForm()
      router.push('/event-list')
    }
    catch (e) {
      submissionStatus.value = { success:false, message:e.message || 'Gagal submit' }
    }
    finally {
      isSubmitting.value = false
    }
  }
  
  function resetForm () {
    eventName.value = ''
    selectedDate.value = null
    dayOfWeek.value = null
    frequency.value  = 'Monthly'
    notes.value      = ''
  }
  </script>
  
  <style scoped>
  .day-content { display: flex; flex-direction: column; align-items: center; }
  .sunday      { color: red; }
  .has-event   { color: #ffeb3b; }
  </style>
  