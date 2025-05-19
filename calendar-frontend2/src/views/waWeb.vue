<template>
    <div class="p-6 max-w-3xl mx-auto">
      <h2 class="text-2xl font-bold mb-4">Kelola Nomor WhatsApp</h2>
  
      <!-- Form tambah nomor -->
      <form @submit.prevent="addNumber" class="mb-6">
        <input
          v-model="newName"
          placeholder="Nama (opsional)"
          class="border p-2 mr-2 rounded"
        />
        <input
          v-model="newPhone"
          placeholder="Nomor WhatsApp (+628...)"
          class="border p-2 mr-2 rounded"
          required
        />
        <button
          type="submit"
          class="bg-green-600 text-white px-4 py-2 rounded hover:bg-green-700"
        >
          Tambah Nomor
        </button>
      </form>
  
      <!-- Tabel nomor -->
      <table class="w-full border-collapse border">
        <thead>
          <tr class="bg-gray-200">
            <th class="border px-4 py-2">ID</th>
            <th class="border px-4 py-2">Nama</th>
            <th class="border px-4 py-2">Nomor WhatsApp</th>
            <th class="border px-4 py-2">Aksi</th>
          </tr>
        </thead>
        <tbody>
            <tr v-for="user in waUsers" :key="user.id" class="hover:bg-gray-100">
            <td class="border px-4 py-2">{{ user.id }}</td>

            <!-- Tampilkan input jika sedang edit -->
            <td class="border px-4 py-2" v-if="editUserId !== user.id">{{ user.name || '-' }}</td>
            <td class="border px-4 py-2" v-else>
                <input v-model="editName" class="border p-1 rounded w-full" />
            </td>

            <td class="border px-4 py-2" v-if="editUserId !== user.id">{{ user.phoneNumber }}</td>
            
            <td class="border px-4 py-2" v-else>
                <input v-model="editPhone" class="border p-1 rounded w-full" />
            </td>

            <td class="border px-4 py-2">
                <template v-if="editUserId === user.id">
                <button
                    @click="saveEdit(user.id)"
                    class="bg-green-600 text-white px-3 py-1 rounded hover:bg-green-700 mr-2"
                >
                    Simpan
                </button>
                <button
                    @click="cancelEdit"
                    class="bg-gray-400 text-white px-3 py-1 rounded hover:bg-gray-500"
                >
                    Batal
                </button>
                </template>
                <template v-else>
                <button
                    @click="startEdit(user)"
                    class="bg-yellow-500 text-white px-3 py-1 rounded hover:bg-yellow-600 mr-2"
                >
                    Edit
                </button>
                <button
                    @click="deleteUser(user.id)"
                    class="bg-red-600 text-white px-3 py-1 rounded hover:bg-red-700"
                >
                    Hapus
                </button>
                </template>
                    <button
                    @click="sendMessage(user.id)"
                    class="bg-blue-600 text-white px-3 py-1 rounded hover:bg-blue-700"
                    >
                    Kirim Pesan
                    </button>
            </td>
            </tr>
        </tbody>
      </table>
  
      <!-- Input pesan -->
      <div v-if="selectedUserId" class="mt-6">
        <h3 class="text-lg font-semibold mb-2">
          Kirim Pesan ke Nomor ID: {{ selectedUserId }}
        </h3>
        <textarea
          v-model="message"
          placeholder="Tulis pesan Anda di sini..."
          class="w-full p-2 border rounded mb-2"
          rows="4"
        ></textarea>
        <button
          @click="confirmSend"
          class="bg-green-600 text-white px-4 py-2 rounded hover:bg-green-700"
          :disabled="isSending"
        >
          {{ isSending ? "Mengirim..." : "Kirim WhatsApp" }}
        </button>
        <button
          @click="cancelSend"
          class="ml-2 bg-gray-400 text-white px-4 py-2 rounded hover:bg-gray-500"
          :disabled="isSending"
        >
          Batal
        </button>
      </div>
  
      <!-- Status -->
      <p v-if="statusMessage" class="mt-4 text-sm text-gray-700">
        {{ statusMessage }}
      </p>
    </div>
  </template>
  
  <script setup>
  import { ref, onMounted } from "vue";
  import axios from "axios";
  
  const API = "http://localhost:5152";
  
  const waUsers = ref([]);
  const newName = ref("");
  const newPhone = ref("");
  const message = ref("");
  const selectedUserId = ref(null);
  const statusMessage = ref("");
  const isSending = ref(false);
  const editUserId = ref(null);
  const editName = ref("");
  const editPhone = ref("");


  async function fetchUsers() {
    try {
      const res = await axios.get(`${API}/wa-users`);
      waUsers.value = res.data;
    } catch {
      alert("Gagal mengambil daftar nomor WA");
    }
  }
  
  // Fungsi normalisasi nomor WA
  function normalizePhoneNumber(input) {
    if (!input) return "";
    let digits = input.replace(/\D/g, ""); // hanya angka
    if (digits.startsWith("0")) digits = "62" + digits.substring(1);
    return digits;
  }
  
  async function addNumber() {
    if (!newPhone.value.trim()) {
      alert("Nomor WhatsApp wajib diisi");
      return;
    }
    const cleanedNumber = normalizePhoneNumber(newPhone.value.trim());
    if (!cleanedNumber.startsWith("62")) {
      alert(
        "Nomor WhatsApp harus diawali kode negara, contoh: 081234567890 menjadi 6281234567890"
      );
      return;
    }
    try {
      await axios.post(`${API}/wa-users`, {
        name: newName.value.trim(),
        phoneNumber: cleanedNumber,
      });
      newName.value = "";
      newPhone.value = "";
      await fetchUsers();
    } catch {
      alert("Gagal menambahkan nomor WA");
    }
  }
  
  function sendMessage(id) {
    selectedUserId.value = id;
    message.value = "";
    statusMessage.value = "";
  }
  
  async function confirmSend() {
    if (!message.value.trim()) {
      alert("Pesan wajib diisi");
      return;
    }
    isSending.value = true;
    statusMessage.value = "";
    try {
      const res = await axios.post(`${API}/send-wa/${selectedUserId.value}`, {
        message: message.value.trim(),
      });
      statusMessage.value =
        "Pesan terkirim! Response: " + JSON.stringify(res.data);
      selectedUserId.value = null;
      message.value = "";
    } catch (e) {
      if (e.response?.data?.Error) {
        statusMessage.value = "Gagal mengirim pesan: " + e.response.data.Error;
      } else {
        statusMessage.value =
          "Gagal mengirim pesan: " + (e.message || "Unknown error");
      }
    } finally {
      isSending.value = false;
    }
  }
  
  function cancelSend() {
    selectedUserId.value = null;
    message.value = "";
    statusMessage.value = "";
  }
  // Mulai edit user
function startEdit(user) {
  editUserId.value = user.id;
  editName.value = user.name || "";
  editPhone.value = user.phoneNumber || "";
}

// Batalkan edit
function cancelEdit() {
  editUserId.value = null;
  editName.value = "";
  editPhone.value = "";
}

// Simpan hasil edit
async function saveEdit(id) {
  if (!editPhone.value.trim()) {
    alert("Nomor WhatsApp wajib diisi");
    return;
  }
  try {
    await axios.put(`${API}/wa-users/${id}`, {
      name: editName.value.trim(),
      phoneNumber: editPhone.value.trim(),
    });
    editUserId.value = null;
    editName.value = "";
    editPhone.value = "";
    await fetchUsers();
  } catch {
    alert("Gagal menyimpan perubahan nomor WA");
  }
}

// Hapus nomor
async function deleteUser(id) {
  if (!confirm("Yakin ingin menghapus nomor ini?")) return;
  try {
    await axios.delete(`${API}/wa-users/${id}`);
    await fetchUsers();
  } catch {
    alert("Gagal menghapus nomor WA");
  }
}

  onMounted(fetchUsers);
  </script>
  