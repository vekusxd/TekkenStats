import { useState, useEffect } from 'react';
import { tekkenApi } from '../api/tekkenApi';

export const usePlayerData = (playerId, filters) => {
  const [playerData, setPlayerData] = useState({
    profile: null,
    matches: [],
    totalMatches: 0,
    opponentCharacters: [],
    rivals: [],
    loading: true,
    error: null
  });

  useEffect(() => {
    const fetchData = async () => {
      try {
        setPlayerData(prev => ({ 
          ...prev, 
          loading: true,
          matches: filters.pageNumber === 1 ? [] : prev.matches
        }));

        const [profileRes, matchupsRes] = await Promise.all([
          tekkenApi.fetchProfile(playerId),
          tekkenApi.fetchMatchups(playerId)
        ]);

        const opponents = matchupsRes.data.map(m => ({
          characterId: m.opponentCharacterId,
          characterName: m.characterName
        })).sort((a, b) => a.characterName.localeCompare(b.characterName));

        const matchesParams = {
          PageSize: filters.pageSize,
          PageNumber: filters.pageNumber,
          CharacterId: filters.characterId,
          OpponentCharacterId: filters.opponentCharacterId
        };

        const rivalsParams = {
          PlayerCharacterId: filters.playerCharacterId,
          OpponentCharacterId: filters.opponentCharacterId
        };

        const [matchesRes, rivalsRes] = await Promise.all([
          tekkenApi.fetchMatches(playerId, matchesParams),
          tekkenApi.fetchRivals(playerId, rivalsParams)
        ]);

        const profilesData = {};
        await Promise.all(
          rivalsRes.data.data.map(async rival => {
            try {
              const profileResponse = await tekkenApi.fetchProfile(rival.tekkenId);
              profilesData[rival.tekkenId] = profileResponse.data;
            } catch (err) {
              console.error(`Error loading profile for ${rival.tekkenId}:`, err);
              profilesData[rival.tekkenId] = { currentName: rival.name };
            }
          })
        );

        setPlayerData(prev => ({
          profile: profileRes.data,
          matches: filters.pageNumber === 1 
            ? matchesRes.data.matches 
            : [...prev.matches, ...matchesRes.data.matches],
          totalMatches: matchesRes.data.totalMatches || 0,
          opponentCharacters: opponents,
          rivals: rivalsRes.data.data,
          rivalsProfiles: profilesData,
          loading: false,
          error: null
        }));
      } catch (err) {
        setPlayerData(prev => ({
          ...prev,
          loading: false,
          error: err.message
        }));
      }
    };

    fetchData();
  }, [playerId, filters]);

  return playerData;
};