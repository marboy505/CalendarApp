<template>
    <div v-show="isVisible" class="fixed inset-0 flex justify-center items-center z-50 bg-black bg-opacity-50 transition-opacity duration-300 ease-in-out">
      <div class="bg-white p-6 rounded-lg shadow-lg w-full sm:w-96 transform transition-all scale-95 opacity-0 animate-popup">
        <h2 class="text-xl font-bold mb-4 text-center">Edit Event</h2>
        <p class="mb-4 text-center">Are you sure you want to edit this event: <strong>{{ event?.eventName }}</strong>?</p>
        <div class="flex justify-end space-x-2">
          <button
            @click="closePopup"
            class="bg-gray-400 text-white font-semibold py-2 px-4 rounded-lg shadow-md hover:bg-gray-500 focus:outline-none focus:ring-2 focus:ring-gray-500 focus:ring-opacity-50"
          >
            Cancel
          </button>
          <button
            @click="confirmEdit"
            class="bg-teal-600 text-white font-semibold py-2 px-4 rounded-lg shadow-md hover:bg-teal-700 focus:outline-none focus:ring-2 focus:ring-teal-500 focus:ring-opacity-50"
          >
            Yes, Edit
          </button>
        </div>
      </div>
    </div>
  </template>
  
  <script setup>
  import { ref, defineProps } from 'vue';
  
  const props = defineProps({
    isVisible: {
      type: Boolean,
      required: true
    },
    event: {
      type: Object,
      required: true
    },
    onConfirm: {
      type: Function,
      required: true
    },
    onClose: {
      type: Function,
      required: true
    }
  });
  
  const confirmEdit = () => {
    props.onConfirm(props.event); // Pass the event data to the parent component
    props.onClose(); // Close the popup
  };
  
  const closePopup = () => {
    props.onClose(); // Close the popup when clicking Cancel
  };
  </script>
  
  <style scoped>
  /* Animasi popup */
  @keyframes popup {
    0% {
      opacity: 0;
      transform: scale(0.8);
    }
    100% {
      opacity: 1;
      transform: scale(1);
    }
  }
  
  .animate-popup {
    animation: popup 0.3s ease-out;
  }
  </style>
  