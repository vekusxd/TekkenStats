import { useState } from 'react';
import { useParams } from 'react-router-dom';
import { useHeadToHead } from './useHeadToHead';

export const useHeadToHeadData = () => {
  const { tekkenId, opponentTekkenId } = useParams();
  const { 
    data: headToHeadData, 
    loading, 
    error,
    matches,
    winCount,
    lossCount,
    winRate
  } = useHeadToHead(tekkenId, opponentTekkenId);
  
  const [yourCharacterFilter, setYourCharacterFilter] = useState('All');
  const [opponentCharacterFilter, setOpponentCharacterFilter] = useState('All');
  const [resultFilter, setResultFilter] = useState('All');

  const filteredMatches = matches.filter(match => {
    const yourCharacterMatch = yourCharacterFilter === 'All' || 
      match.challenger.characterName === yourCharacterFilter;
    const opponentCharacterMatch = opponentCharacterFilter === 'All' || 
      match.opponent.characterName === opponentCharacterFilter;
    const resultMatch = resultFilter === 'All' || 
      (resultFilter === 'Win' && match.winner) || 
      (resultFilter === 'Loss' && !match.winner);
    return yourCharacterMatch && opponentCharacterMatch && resultMatch;
  });

  const playerName = matches[0]?.challenger.name || 'Player';
  const opponentName = matches[0]?.opponent.name || 'Opponent';

  const getLatestRating = (isChallenger) => {
    for (let i = matches.length - 1; i >= 0; i--) {
      const rating = isChallenger 
        ? matches[i].challenger.ratingBefore 
        : matches[i].opponent.ratingBefore;
      if (rating !== 0) return rating;
    }
    return 0;
  };

  const playerRating = getLatestRating(true);
  const opponentRating = getLatestRating(false);

  const yourCharacters = [...new Set(matches.map(m => m.challenger.characterName))];
  const opponentCharacters = [...new Set(matches.map(m => m.opponent.characterName))];

  return {
    loading,
    error,
    headToHeadData,
    filteredMatches,
    winCount,
    lossCount,
    winRate,
    playerName,
    opponentName,
    playerRating,
    opponentRating,
    yourCharacters,
    opponentCharacters,
    yourCharacterFilter,
    setYourCharacterFilter,
    opponentCharacterFilter,
    setOpponentCharacterFilter,
    resultFilter,
    setResultFilter
  };
};