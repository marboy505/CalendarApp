import { createStore } from 'vuex';

// Create a new Vuex store
const store = createStore({
  state() {
    return {
      currentPage: 'event-entry', // Default page
      events: [
        { id: 'W1', eventName: 'Team Stand-up Meeting', frequency: 'Weekly', notes: 'Every Monday' },
        { id: 'M1', eventName: 'Rent Payment', frequency: 'Monthly', notes: '1st of month' },
        { id: 'M2', eventName: 'Marketing Newsletter', frequency: 'Monthly', notes: '1st Tuesday' },
        { id: 'Y1', eventName: 'Company Anniversary', frequency: 'Yearly', notes: '15 June' },
        { id: 'Y2', eventName: 'Domain Renewal', frequency: 'Yearly', notes: '30 September' },
      ]
    };
  },
  mutations: {
    setCurrentPage(state, page) {
      state.currentPage = page;
    },
    addEvent(state, event) {
      state.events.push(event);
    },
  },
  actions: {
    navigateToPage({ commit }, page) {
      commit('setCurrentPage', page);  // Commit mutation to change currentPage
    },
    createEvent({ commit }, event) {
      commit('addEvent', event);  // Commit mutation to add a new event
    },
  },
  getters: {
    currentPage: (state) => state.currentPage,
    allEvents: (state) => state.events,
  },
});

export default store;
