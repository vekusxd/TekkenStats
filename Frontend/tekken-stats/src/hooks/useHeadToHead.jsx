import { useState, useEffect } from 'react';
import { tekkenApi } from '../api/tekkenApi';

export const useHeadToHead = (playerId, opponentId) => {
  const [data, setData] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchData = async () => {
      try {
        setLoading(true);
        const response = await tekkenApi.fetchHeadToHead(playerId, opponentId);
        setData(response.data);
        setLoading(false);
      } catch (err) {
        setError(err.message);
        setLoading(false);
      }
    };

    if (playerId && opponentId) {
      fetchData();
    }
  }, [playerId, opponentId]);

  return { 
    data, 
    loading, 
    error,
    matches: data?.matches || [],
    winCount: data?.winCount || 0,
    lossCount: data?.lossCount || 0,
    winRate: data?.challengerWinRate || 0
  };
};