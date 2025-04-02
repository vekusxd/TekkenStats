import { useState } from 'react';
import { tekkenApi } from '../api/tekkenApi';

export const usePlayerSearch = () => {
  const [searchState, setSearchState] = useState({
    loading: false,
    error: null,
    results: []
  });

  const validateQuery = (query) => {
    if (!query || !query.trim()) {
      throw new Error('Please enter a player name');
    }
    if (/[!@#$%^&*()_+=<>?/\\[\]{}|]/.test(query)) {
      throw new Error('Name should not contain special characters');
    }
  };

  const searchPlayers = async (query) => {
    try {
      validateQuery(query);
      
      setSearchState({ loading: true, error: null, results: [] });
      const response = await tekkenApi.searchPlayers(query.trim());
      setSearchState({
        loading: false,
        error: null,
        results: response.data
      });
      return response.data;
    } catch (error) {
      setSearchState({
        loading: false,
        error: error.message,
        results: []
      });
      throw error;
    }
  };

  return {
    ...searchState,
    searchPlayers
  };
};