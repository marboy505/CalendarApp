<template>
    <div class="calendar-container">
      <!-- ===== Navigasi bulan ===== -->
      <div class="month-navigation">
        <button @click="changeMonth(-1)" class="nav-button-left">←</button>
        <div class="month-year-header">
          <span class="month-name">{{ monthName }}</span>
          <span class="year">{{ year }}</span>
        </div>
        <button @click="changeMonth(1)" class="nav-button-right">→</button>
      </div>
  
      <!-- ===== Header hari ===== -->
      <div class="weekdays">
        <div v-for="d in days" :key="d" class="weekday">{{ d }}</div>
      </div>
  
      <!-- ===== Grid tanggal ===== -->
      <div class="calendar-days">
        <div v-for="(date, i) in monthDays" :key="i" class="calendar-day">
          <div
            v-if="date"
            class="day-number"
            :class="{
              'current-day': isToday(date),
              sunday: isSunday(date),
              saturday: isSaturday(date)
            }"
            @click="selectDate(date)"
          >
            {{ date }}
          </div>
          <div v-else class="day-number"></div>
  
          <!-- titik warna -->
          <div v-if="frequenciesOf(date).length" class="event-dots">
            <span
              v-for="f in frequenciesOf(date)"
              :key="f"
              :class="['dot', 'dot-' + f.toLowerCase()]"
            ></span>
          </div>
        </div>
      </div>
  
      <!-- ===== Panel detail ===== -->
      <div v-if="panel.open" class="event-details">
        <h3 class="details-title">
          {{ panel.label }}
        </h3>
  
        <template v-if="panel.events.length">
          <div
            v-for="e in panel.events"
            :key="e.eventId + '-' + e.frequency"
            class="event-item"
          >
            • <strong>{{ e.eventName }}</strong>
            ({{ e.frequency }}) <span v-if="e.notes">— {{ e.notes }}</span>
          </div>
        </template>
  
        <p v-else class="no-event">Tidak ada event</p>
      </div>
    </div>
  </template>
  
  <script setup>
  import { ref, onMounted } from 'vue'
  import axios from 'axios'
  
  /* ---------- state ---------- */
  const events         = ref([]) // tabel Events
  const weeklyEntries  = ref([]) // WeeklyEventEntries
  const monthlyEntries = ref([]) // MonthlyEventEntries
  const yearlyEntries  = ref([]) // YearlyEventEntries
  
  const currentDate = ref(new Date())          // penanda bulan yg sedang dilihat
  const monthName   = ref('')
  const year        = ref(0)
  const monthDays   = ref([])
  
  const days = ['S', 'M', 'T', 'W', 'T', 'F', 'S']
  
  /* panel detail */
  const panel = ref({
    open   : false,
    label  : '',
    events : []
  })
  
  /* ---------- fetch ---------- */
  async function loadData () {
    const base = 'http://localhost:5152'
    const [eRes, wRes, mRes, yRes] = await Promise.all([
      axios.get(`${base}/events?page=1&pageSize=1000`),
      axios.get(`${base}/weekly-events`),
      axios.get(`${base}/monthly-events`),
      axios.get(`${base}/yearly-events`)
    ])
  
    events.value         = eRes.data.events || eRes.data.Events || []
    weeklyEntries.value  = wRes.data
    monthlyEntries.value = mRes.data
    yearlyEntries.value  = yRes.data
  }
  
  /* ---------- kalender util ---------- */
  function buildMonth () {
    const first = new Date(year.value, currentDate.value.getMonth(), 1)
    const last  = new Date(year.value, currentDate.value.getMonth() + 1, 0)
  
    monthDays.value = [
      ...Array(first.getDay()).fill(''),
      ...Array.from({ length: last.getDate() }, (_, i) => i + 1)
    ]
    monthName.value = currentDate.value.toLocaleString('default', { month: 'long' })
    year.value      = currentDate.value.getFullYear()
  }
  
  function isToday (d) {
    const t = new Date()
    return d &&
           t.getDate()   === d &&
           t.getMonth()  === currentDate.value.getMonth() &&
           t.getFullYear() === year.value
  }
  const isSunday   = d => d && new Date(year.value, currentDate.value.getMonth(), d).getDay() === 0
  const isSaturday = d => d && new Date(year.value, currentDate.value.getMonth(), d).getDay() === 6
  
  /* ---------- aturan dot & detail ---------- */
  function frequenciesOf (d) {
    if (!d) return []
  
    const m = currentDate.value.getMonth() + 1 // 1-based
    const y = year.value
  
    const freqSet = new Set()
  
    // weekly (bandingkan nama hari)
    const dow = ['Sun','Mon','Tue','Wed','Thu','Fri','Sat'][new Date(y, m-1, d).getDay()]
    weeklyEntries.value.forEach(w => { if (w.dayOfWeek === dow) freqSet.add('Weekly') })
  
    // monthly
    monthlyEntries.value.forEach(mo => {
      if (mo.dayOfMonth === d) freqSet.add('Monthly')
    })
  
    // yearly
    yearlyEntries.value.forEach(ye => {
      if (ye.dayOfMonth === d && monthIndex(ye.month) === m) freqSet.add('Yearly')
    })
  
    // event single‐date (mis. one-shot)
    events.value.forEach(e => {
      if (!e.eventDate) return
      const dt = new Date(e.eventDate)
      if (dt.getDate() === d && dt.getMonth()+1 === m && dt.getFullYear() === y)
        freqSet.add(e.frequency)
    })
  
    return [...freqSet]
  }
  
  function selectDate (d) {
    if (!d) return
    const label = `${String(d).padStart(2,'0')} ${monthName.value} ${year.value}`
  
    const m = currentDate.value.getMonth() + 1
    const y = year.value
  
    const list = []
  
    // dari Events (Monthly/Yearly yang disimpan langsung dll.)
    events.value.forEach(e => {
      if (!e.eventDate) return
      const dt = new Date(e.eventDate)
      if (dt.getDate() === d && dt.getMonth()+1 === m && dt.getFullYear() === y) list.push(e)
    })
  
    // weekly
    const dow = ['Sun','Mon','Tue','Wed','Thu','Fri','Sat'][new Date(y, m-1, d).getDay()]
    weeklyEntries.value
      .filter(w => w.dayOfWeek === dow)
      .forEach(w => {
        const ev = events.value.find(e => e.eventId === w.eventId)
        if (ev) list.push(ev)
      })
  
    // monthly
    monthlyEntries.value
      .filter(mo => mo.dayOfMonth === d)
      .forEach(mo => {
        const ev = events.value.find(e => e.eventId === mo.eventId)
        if (ev) list.push(ev)
      })
  
    // yearly
    yearlyEntries.value
      .filter(ye => ye.dayOfMonth === d && monthIndex(ye.month) === m)
      .forEach(ye => {
        const ev = events.value.find(e => e.eventId === ye.eventId)
        if (ev) list.push(ev)
      })
  
    panel.value = { open:true, label, events:list }
  }
  
  function changeMonth (dir) {
    currentDate.value.setMonth(currentDate.value.getMonth() + dir)
    panel.value.open = false
    buildMonth()
  }
  
  const monthIndex = (monStr) =>
    ['Jan','Feb','Mar','Apr','May','Jun','Jul','Aug','Sep','Oct','Nov','Dec'].indexOf(monStr) + 1
  
  /* ---------- init ---------- */
  onMounted(async () => {
    await loadData()
    buildMonth()
  })
  </script>
  
  <style scoped>
  /* ===== style lama ===== */
  .calendar-container{padding:20px;background:#051408;border-radius:8px}
  .month-navigation{display:flex;justify-content:space-between;align-items:center;margin-bottom:20px}
  .nav-button-left,.nav-button-right{background:#051408;color:#fcf5f5;padding:10px;border-radius:8px;cursor:pointer;transition:.3s}
  .nav-button-left:hover,.nav-button-right:hover{background:#fcf5f5;color:#051408}
  .month-year-header{display:flex;align-items:center;font-size:1.5rem;color:#fff}
  .month-name{font-weight:bold}.year{margin-left:10px}
  .weekdays{display:grid;grid-template-columns:repeat(7,1fr);text-align:center;margin-bottom:10px;font-weight:bold}
  .weekday{color:#d1d5db}
  .calendar-days{display:grid;grid-template-columns:repeat(7,1fr);gap:10px}
  .calendar-day{position:relative;display:flex;justify-content:center}
  .day-number{width:36px;height:36px;line-height:36px;text-align:center;border-radius:50%;color:#d1d5db;cursor:pointer;transition:.3s}
  .day-number:hover{background:#fcf5f5;color:#051408}
  .day-number.current-day{background:#f97316;color:#fff}
  .day-number.sunday,.day-number.saturday{color:red}
  
  /* ===== dots by frequency ===== */
  .event-dots{position:absolute;bottom:4px;left:50%;transform:translateX(-50%);display:flex;gap:2px}
  .dot{width:6px;height:6px;border-radius:50%}
  .dot-weekly  {background:#10b981}  /* hijau */
  .dot-monthly {background:#ffeb3b}  /* kuning */
  .dot-yearly  {background:#ef4444}  /* merah */
  
  /* ===== panel detail ===== */
  .event-details{margin-top:20px;background:#374151;padding:15px;border-radius:8px}
  .details-title{color:#fff;margin-bottom:10px}
  .event-item{color:#d1d5db;margin-bottom:4px}
  .no-event{color:#9ca3af}
  </style>
  