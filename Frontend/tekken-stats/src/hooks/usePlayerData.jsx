import { useState, useEffect } from 'react';
import { tekkenApi } from '../api/tekkenApi';

export const usePlayerData = (playerId) => {
  const [playerData, setPlayerData] = useState({
    profile: null,
    matches: [],
    opponentCharacters: [],
    loading: true,
    error: null
  });

  useEffect(() => {
    const fetchData = async () => {
      try {
        const [profileRes, matchesRes, matchupsRes] = await Promise.all([
          tekkenApi.fetchProfile(playerId),
          tekkenApi.fetchMatches(playerId),
          tekkenApi.fetchMatchups(playerId)
        ]);

        const opponents = matchupsRes.data.map(m => ({
          characterId: m.opponentCharacterId,
          characterName: m.characterName
        })).sort((a, b) => a.characterName.localeCompare(b.characterName));

        setPlayerData({
          profile: profileRes.data,
          matches: matchesRes.data.matches,
          opponentCharacters: opponents,
          loading: false,
          error: null
        });
      } catch (err) {
        setPlayerData(prev => ({
          ...prev,
          loading: false,
          error: err.message
        }));
      }
    };

    fetchData();
  }, [playerId]);

  return playerData;
};