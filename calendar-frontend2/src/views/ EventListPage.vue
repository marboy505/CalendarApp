<!-- src/pages/EventListPage.vue -->
<template>
    <div class="min-h-screen py-12 px-6 sm:px-8 lg:px-10">
      <div class="max-w-7xl mx-auto">
        <!-- ===== Header ===== -->
        <header class="text-center mb-8">
          <h1 class="text-4xl font-extrabold text-gray-800">Event List</h1>
          <p class="mt-4 text-xl text-gray-600">Kelola event Anda dengan mudah</p>
        </header>
  
        <!-- ===== Toolbar ===== -->
        <div
          class="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4 mb-6"
        >
          <!-- Search -->
          <div class="flex-1 relative">
            <input
              v-model="searchQuery"
              type="text"
              placeholder="Cari nama / frequency…"
              class="w-full py-2 pl-10 pr-4 rounded-lg shadow focus:ring-2 focus:ring-teal-500 focus:outline-none"
            />
            <span
              class="absolute inset-y-0 left-0 flex items-center pl-3 text-gray-400 material-symbols-outlined"
            >
              search
            </span>
          </div>
  
          <!-- Filter -->
          <select
            v-model="frequencyFilter"
            class="w-full sm:w-48 py-2 px-3 rounded-lg shadow focus:ring-2 focus:ring-teal-500 focus:outline-none"
          >
            <option value="">Semua Frequency</option>
            <option v-for="f in uniqueFrequencies" :key="f" :value="f">{{ f }}</option>
          </select>
  
          <!-- Add -->
          <router-link
            to="/event-entry"
            class="flex items-center gap-1 bg-green-600 text-white font-semibold py-2 px-4 rounded-lg shadow hover:bg-green-700 focus:outline-none focus:ring-2 focus:ring-green-500"
          >
            <span class="material-symbols-outlined text-base">add_circle</span>
            Add Event
          </router-link>
        </div>
  
        <!-- ===== Card ===== -->
        <div class="relative overflow-hidden bg-white shadow-lg rounded-lg">
          <!-- Header -->
          <div
            class="py-4 px-6 bg-gray-50 flex justify-between items-center space-x-4"
          >
            <button
              @click="refresh"
              :disabled="loading"
              class="bg-indigo-600 text-white font-semibold py-2 px-4 rounded-lg shadow hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-indigo-500 disabled:opacity-50"
            >
              {{ loading ? 'Memuat…' : 'Muat Ulang' }}
            </button>
            <span class="text-sm text-gray-500"
              >Total: {{ totalCount }} | Hal. {{ currentPage }} /
              {{ totalPages }}</span
            >
          </div>
  
          <!-- Spinner overlay -->
          <div
            v-if="loading"
            class="absolute inset-0 flex items-center justify-center bg-white/70 z-10"
          >
            <svg
              class="animate-spin h-8 w-8 text-teal-600"
              xmlns="http://www.w3.org/2000/svg"
              fill="none"
              viewBox="0 0 24 24"
            >
              <circle
                class="opacity-25"
                cx="12"
                cy="12"
                r="10"
                stroke="currentColor"
                stroke-width="4"
              />
              <path
                class="opacity-75"
                fill="currentColor"
                d="M4 12a8 8 0 018-8v4a4 4 0 00-4 4H4z"
              />
            </svg>
          </div>
  
          <!-- ===== Table ===== -->
          <div class="overflow-x-auto">
            <table class="min-w-full divide-y divide-gray-200">
              <thead class="bg-teal-100">
                <tr>
                  <th
                    class="px-6 py-3 text-left text-xs font-medium text-gray-600 uppercase"
                  >
                    Event
                  </th>
                  <th
                    class="px-6 py-3 text-left text-xs font-medium text-gray-600 uppercase"
                  >
                    Frequency
                  </th>
                  <th
                    class="px-6 py-3 text-left text-xs font-medium text-gray-600 uppercase"
                  >
                    Tanggal Event
                  </th>
                  <th
                    class="px-6 py-3 text-center text-xs font-medium text-gray-600 uppercase"
                  >
                    Aksi
                  </th>
                </tr>
              </thead>
  
              <tbody class="bg-white divide-y divide-gray-200">
                <tr
                  v-for="evt in paginatedEvents"
                  :key="evt.eventId"
                  class="hover:bg-gray-50"
                >
                  <td
                    class="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900"
                  >
                    {{ evt.eventName }}
                  </td>
                  <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-700">
                    {{ evt.frequency }}
                  </td>
                  <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-700">
                    {{ evt.timeLabel || 'Tidak tersedia' }}
                  </td>
  
                  <!-- aksi -->
                  <td class="px-6 py-4 whitespace-nowrap text-sm">
                    <div class="flex justify-center gap-3">
                      <button
                        @click="openEdit(evt)"
                        class="flex items-center gap-1 px-3 py-1 rounded-md shadow bg-green-600 text-white hover:bg-green-700 focus:ring-2 focus:ring-green-400"
                      >
                        <span class="material-symbols-outlined text-sm">edit</span>
                        Edit
                      </button>
  
                      <button
                        @click="openDelete(evt)"
                        class="flex items-center gap-1 px-3 py-1 rounded-md shadow bg-red-600 text-white hover:bg-red-700 focus:ring-2 focus:ring-red-400"
                      >
                        <span class="material-symbols-outlined text-sm">delete</span>
                        Delete
                      </button>
                    </div>
                  </td>
                </tr>
  
                <tr v-if="!loading && paginatedEvents.length === 0">
                  <td colspan="4" class="py-6 text-center text-gray-500">
                    Tidak ada data
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
  
          <!-- ===== Pagination ===== -->
          <nav
            v-if="totalPages > 1"
            class="flex justify-center items-center gap-2 py-4 bg-gray-50"
          >
            <button
              @click="goToPage(currentPage - 1)"
              :disabled="currentPage === 1"
              class="px-3 py-1 rounded-lg shadow text-sm font-semibold bg-white hover:bg-teal-50 disabled:opacity-50"
            >
              « Prev
            </button>
  
            <button
              v-for="n in pageNumbers"
              :key="n"
              @click="goToPage(n)"
              :class="[
                'px-3 py-1 rounded-lg shadow text-sm font-semibold',
                n === currentPage ? 'bg-teal-600 text-white' : 'bg-white hover:bg-teal-50'
              ]"
            >
              {{ n }}
            </button>
  
            <button
              @click="goToPage(currentPage + 1)"
              :disabled="currentPage === totalPages"
              class="px-3 py-1 rounded-lg shadow text-sm font-semibold bg-white hover:bg-teal-50 disabled:opacity-50"
            >
              Next »
            </button>
          </nav>
        </div>
      </div>
  
      <!-- ===== MODAL EDIT ===== -->
      <div
        v-if="showEdit"
        class="fixed inset-0 z-50 flex items-center justify-center bg-black/40"
      >
        <div class="bg-white w-full max-w-md mx-4 p-6 rounded-lg shadow-lg">
          <h2 class="text-xl font-bold mb-4 text-green-700 text-center">
            Edit Event
          </h2>
  
          <!-- Nama -->
          <label class="block text-sm font-medium mb-1">Nama Event</label>
          <input
            v-model="editData.eventName"
            class="w-full mb-3 py-2 px-3 border rounded-md focus:ring-2 focus:ring-green-500"
          />
  
          <!-- Frequency -->
          <label class="block text-sm font-medium mb-1">Frequency</label>
          <select
            v-model="editData.frequency"
            class="w-full mb-3 py-2 px-3 border rounded-md focus:ring-2 focus:ring-green-500"
          >
            <option>Weekly</option>
            <option>Monthly</option>
            <option>Yearly</option>
          </select>
  
          <!-- Dynamic input -->
          <template v-if="editData.frequency === 'Weekly'">
            <label class="block text-sm font-medium mb-1"
              >Hari (boleh multi-pilih)</label
            >
            <select
              v-model="editData.weekDays"
              multiple
              class="w-full h-32 mb-3 py-2 px-3 border rounded-md focus:ring-2 focus:ring-green-500"
            >
              <option v-for="d in weekDays" :key="d" :value="d">{{ d }}</option>
            </select>
          </template>
  
          <template v-else-if="editData.frequency === 'Monthly'">
            <label class="block text-sm font-medium mb-1">Tanggal (1-31)</label>
            <input
              type="number"
              min="1"
              max="31"
              v-model.number="editData.dayOfMonth"
              class="w-full mb-3 py-2 px-3 border rounded-md focus:ring-2 focus:ring-green-500"
            />
          </template>
  
          <template v-else>
            <label class="block text-sm font-medium mb-1">Tanggal + Bulan</label>
            <input
              type="date"
              v-model="editData.eventDate"
              class="w-full mb-3 py-2 px-3 border rounded-md focus:ring-2 focus:ring-green-500"
            />
          </template>
  
          <!-- Notes -->
          <label class="block text-sm font-medium mb-1">Catatan</label>
          <textarea
            v-model="editData.notes"
            class="w-full mb-4 py-2 px-3 border rounded-md focus:ring-2 focus:ring-green-500"
          ></textarea>
  
          <!-- buttons -->
          <div class="flex justify-end gap-2">
            <button
              @click="showEdit = false"
              class="px-4 py-2 rounded-md bg-gray-400 text-white hover:bg-gray-500"
            >
              Cancel
            </button>
            <button
              @click="update"
              class="px-4 py-2 rounded-md bg-green-600 text-white hover:bg-green-700"
            >
              Submit
            </button>
          </div>
        </div>
      </div>
  
      <!-- ===== MODAL DELETE ===== -->
      <div
        v-if="showDelete"
        class="fixed inset-0 z-50 flex items-center justify-center bg-black/40"
      >
        <div class="bg-white w-full max-w-sm mx-4 p-6 rounded-lg shadow-lg">
          <h2 class="text-xl font-bold mb-4 text-red-600 text-center">
            Hapus Event?
          </h2>
          <p class="text-center mb-6">
            Anda yakin ingin menghapus
            <strong>{{ deleteTarget?.eventName }}</strong>?
          </p>
          <div class="flex justify-end gap-2">
            <button
              @click="showDelete = false"
              class="px-4 py-2 rounded-md bg-gray-400 text-white hover:bg-gray-500"
            >
              Cancel
            </button>
            <button
              @click="remove"
              class="px-4 py-2 rounded-md bg-red-600 text-white hover:bg-red-700"
            >
              Delete
            </button>
          </div>
        </div>
      </div>
    </div>
  </template>
  
  <script setup>
  import { ref, computed, watch, onMounted } from 'vue'
  import axios from 'axios'
  
  /* ===== constants ===== */
  const API = 'http://localhost:5152'
  const weekDays = ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun']
  const monthShort = (d) => d.toLocaleString('en-US', { month: 'short' })
  
  /* ===== state ===== */
  const events = ref([])
  const totalCount = ref(0)
  const loading = ref(false)
  
  const searchQuery = ref('')
  const frequencyFilter = ref('')
  
  const currentPage = ref(1)
  const pageSize = ref(10)
  const totalPages = computed(() =>
    Math.max(1, Math.ceil(totalCount.value / pageSize.value))
  )
  
  /* ===== modal ===== */
  const showEdit = ref(false)
  const showDelete = ref(false)
  const editData = ref(newEditData())
  const deleteTarget = ref(null)
  
  /* ===== fetch list ===== */
  async function fetchEvents() {
    loading.value = true
    try {
      const { data } = await axios.get(`${API}/events`, {
        params: { page: currentPage.value, pageSize: pageSize.value }
      })
      events.value = data.events ?? data.Events ?? []
      totalCount.value = data.totalCount ?? 0
    } catch (e) {
      console.error(e)
      alert('Gagal memuat event')
    } finally {
      loading.value = false
    }
  }
  
  /* ===== filtering ===== */
  const uniqueFrequencies = computed(() =>
    [...new Set(events.value.map((e) => e.frequency))].sort()
  )
  const filtered = computed(() => {
    let list = events.value
    if (searchQuery.value) {
      const q = searchQuery.value.toLowerCase()
      list = list.filter(
        (e) =>
          e.eventName.toLowerCase().includes(q) ||
          e.frequency.toLowerCase().includes(q)
      )
    }
    if (frequencyFilter.value)
      list = list.filter((e) => e.frequency === frequencyFilter.value)
    return list
  })
  const paginatedEvents = computed(() => filtered.value)
  const pageNumbers = computed(() =>
    Array.from({ length: totalPages.value }, (_, i) => i + 1)
  )
  
  /* ===== actions ===== */
  function goToPage(n) {
    if (n < 1 || n > totalPages.value) return
    currentPage.value = n
    fetchEvents()
  }
  const refresh = () => fetchEvents()
  
  /* ===== EDIT ===== */
  function newEditData() {
    return {
      eventId: null,
      eventName: '',
      frequency: 'Weekly',
      weekDays: [],
      dayOfMonth: 1,
      eventDate: '',
      notes: ''
    }
  }
  function openEdit(evt) {
    const obj = newEditData()
    Object.assign(obj, {
      eventId: evt.eventId,
      eventName: evt.eventName,
      frequency: evt.frequency,
      notes: evt.notes || ''
    })
    if (evt.frequency === 'Weekly') {
      obj.weekDays = evt.timeLabel?.split(',').map((d) => d.trim()) ?? []
    } else if (evt.frequency === 'Monthly') {
      obj.dayOfMonth = Number(evt.timeLabel) || 1
    } else {
      obj.eventDate =
        evt.eventDate?.substring(0, 10) ??
        `${new Date().getFullYear()}-01-01`
    }
    editData.value = obj
    showEdit.value = true
  }
  
  /* submit */
  async function update() {
    if (!editData.value.eventName.trim()) {
      alert('Nama event wajib diisi'); return
    }
    try {
      /* 1) update tabel Events */
      await axios.put(`${API}/events/${editData.value.eventId}`, {
        eventName: editData.value.eventName,
        frequency: editData.value.frequency,
        notes: editData.value.notes,
        eventDate:
          editData.value.frequency === 'Weekly'
            ? null
            : editData.value.eventDate
              ? new Date(editData.value.eventDate).toISOString()
              : null
      })

// ------------------------- WEEKLY -------------------------
if (editData.value.frequency === 'Weekly') {
  // --- reset entri lama
  await axios.delete(`${API}/weekly-events/${editData.value.eventId}`)

  // --- sisipkan entri baru (jika user memilih hari)
  if (editData.value.weekDays?.length) {
    await axios.post(`${API}/weekly-events`, {
      eventId   : editData.value.eventId,
      dayOfWeek : editData.value.weekDays.join(','),   // "Mon,Thu"
      notes     : editData.value.notes || null
    })
  }
}

// ------------------------- MONTHLY ------------------------
else if (editData.value.frequency === 'Monthly') {
  await axios.delete(`${API}/monthly-events/${editData.value.eventId}`)

  // pastikan dayOfMonth terisi (1-31)
  if (editData.value.dayOfMonth) {
    const d = new Date(editData.value.eventDate ?? new Date())
    await axios.post(`${API}/monthly-events`, {
      eventId    : editData.value.eventId,
      dayOfMonth : editData.value.dayOfMonth,          // angka 1-31
      month      : monthShort(d),                      // “May”
      notes      : editData.value.notes || null
    })
  }
}

// ------------------------- YEARLY -------------------------
else if (editData.value.frequency === 'Yearly') {
  await axios.delete(`${API}/yearly-events/${editData.value.eventId}`)

  if (editData.value.eventDate) {
    const d = new Date(editData.value.eventDate)
    await axios.post(`${API}/yearly-events`, {
      eventId    : editData.value.eventId,
      month      : monthShort(d),                      // “May”
      dayOfMonth : d.getDate(),                        // 1-31
      notes      : editData.value.notes || null
    })
  }
}

  
      showEdit.value = false
      await fetchEvents()
      alert('Event berhasil di-update')
    } catch (e) {
      console.error(e)
      alert('Gagal update event')
    }
  }
  
  /* ===== DELETE ===== */
  function openDelete(evt) {
    deleteTarget.value = evt
    showDelete.value = true
  }
  async function remove() {
    try {
      await axios.delete(`${API}/events/${deleteTarget.value.eventId}`)
      showDelete.value = false
      await fetchEvents()
      alert('Event dihapus')
    } catch (e) {
      console.error(e)
      alert('Gagal hapus event')
    }
  }
  
  /* ===== watch ===== */
  watch(
    () => editData.value.frequency,
    (f) => {
      if (f === 'Weekly') {
        editData.value.weekDays = []
      } else if (f === 'Monthly') {
        editData.value.dayOfMonth = 1
      } else {
        editData.value.eventDate = ''
      }
    }
  )
  
  /* ===== init ===== */
  onMounted(fetchEvents)
  </script>
  