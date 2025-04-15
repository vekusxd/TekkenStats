import axios from 'axios';
import { BASE_URL } from '../config/baseUrl';

export const tekkenApi = {
    fetchProfile: (playerId) => axios.get(`${BASE_URL}/api/profile/${playerId}`),
    fetchMatches: (playerId, params) => axios.get(`${BASE_URL}/api/matches/${playerId}`, { params }),
    fetchMatchups: (playerId, params) => axios.get(`${BASE_URL}/api/matchups/${playerId}`, { params }),
    fetchRivals: (playerId, params) => axios.get(`${BASE_URL}/api/rivals/${playerId}`, { params }),
    fetchHeadToHead: (playerId, opponentId) => axios.get(`${BASE_URL}/api/head-to-head/${playerId}/${opponentId}`),
    searchPlayers: (query) => axios.get(`${BASE_URL}/api/names`, { params: { StartsWith: query } })
  };