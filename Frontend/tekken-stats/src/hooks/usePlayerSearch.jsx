import { useState, useCallback } from 'react';
import { tekkenApi } from '../api/tekkenApi';

export const usePlayerSearch = () => {
  const [searchState, setSearchState] = useState({
    loading: false,
    error: null,
    results: []
  });

  const validateQuery = useCallback((query) => {
    if (!query || !query.trim()) {
      throw new Error('Please enter a player name');
    }
    if (/[!@#$%^&*()_+=<>?/\\[\]{}|]/.test(query)) {
      throw new Error('Name should not contain special characters');
    }
  }, []);

  const searchPlayers = useCallback(async (query) => {
    try {
      validateQuery(query);
      
      setSearchState(prev => ({ ...prev, loading: true, error: null, results: [] }));
      const response = await tekkenApi.searchPlayers(query.trim());
      setSearchState({
        loading: false,
        error: null,
        results: response.data
      });
      return response.data;
    } catch (error) {
      setSearchState(prev => ({
        ...prev,
        loading: false,
        error: error.message,
        results: []
      }));
      throw error;
    }
  }, [validateQuery]);

  return {
    ...searchState,
    searchPlayers
  };
};