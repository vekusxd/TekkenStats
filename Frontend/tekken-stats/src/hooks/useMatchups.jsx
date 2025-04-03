import { useState, useEffect } from 'react';
import { tekkenApi } from '../api/tekkenApi';

export const useMatchups = (tekkenId, playerCharacterId) => {
  const [matchups, setMatchups] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchMatchups = async () => {
      try {
        setLoading(true);
        const response = await tekkenApi.fetchMatchups(tekkenId, {
          playerCharacterId: playerCharacterId === 'All Characters' ? null : playerCharacterId
        });
        setMatchups(response.data);
        setError(null);
      } catch (err) {
        setError(err.message);
        setMatchups([]);
      } finally {
        setLoading(false);
      }
    };

    if (tekkenId) {
      fetchMatchups();
    }
  }, [tekkenId, playerCharacterId]);

  return { matchups, loading, error };
};