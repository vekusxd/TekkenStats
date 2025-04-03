import { useState, useEffect, useCallback } from 'react';
import { usePlayerSearch } from './usePlayerSearch';

export const useInlineSearch = () => {
  const [searchQuery, setSearchQuery] = useState('');
  const [inputError, setInputError] = useState(null);
  const [suggestions, setSuggestions] = useState([]);
  const [showSuggestions, setShowSuggestions] = useState(false);
  const { searchPlayers, loading } = usePlayerSearch();

  const handleInputChange = useCallback((value) => {
    setSearchQuery(value);
    setInputError(null);
  }, []);

  const performSearch = useCallback(async (query) => {
    try {
      const results = await searchPlayers(query);
      setSuggestions(results);
      setShowSuggestions(true);
    } catch (error) {
      setInputError(error.message);
      setSuggestions([]);
    }
  }, [searchPlayers]);

  useEffect(() => {
    if (searchQuery.trim().length > 0) {
      const timer = setTimeout(() => {
        performSearch(searchQuery);
      }, 300);
      
      return () => clearTimeout(timer);
    } else {
      setSuggestions([]);
      setShowSuggestions(false);
    }
  }, [searchQuery, performSearch]);

  return {
    searchQuery,
    inputError,
    suggestions,
    showSuggestions,
    loading,
    searchPlayers,
    handleInputChange,
    setSearchQuery,
    setShowSuggestions,
    setInputError
  };
};