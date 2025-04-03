import React, { useState } from 'react';
import { useParams } from 'react-router-dom';
import styles from '../styles/TekkenStatsProfile.module.css';
import Header from './Header';
import ProfileSummary from './profile/ProfileSummary';
import MatchHistory from './profile/MatchHistory';
import NameHistory from './profile/NameHistory';
import CharacterStatistics from './profile/CharacterStatistics';
import Rivals from './profile/Rivals';
import Matchups from './profile/Matchups';
import { usePlayerData } from '../hooks/usePlayerData';

const TekkenStatsProfile = () => {
  const { tekkenId } = useParams();
  const [activeTab, setActiveTab] = useState('matchHistory');
  const [filters, setFilters] = useState({
    playerCharacterId: null,
    characterId: null,
    opponentCharacterId: null,
    pageSize: 10,
    pageNumber: 1
  });

  const { 
    profile, 
    matches, 
    rivals, 
    rivalsProfiles, 
    opponentCharacters, 
    error 
  } = usePlayerData(tekkenId, filters);

  if (error) return <div className={styles.error}>Error: {error}</div>;
  if (!profile) return <div className={styles.noData}></div>;

  return (
    <div className={styles.app}>
      <Header />
      <main className={`${styles.container} ${styles.main}`}>
        <div className={styles.content}>
          <ProfileSummary profile={profile} />
          
          <div className={styles.columns}>
            <div className={styles.leftColumn}>
              <div className={styles.tabsContainer}>
                <div className={styles.tabsHeader}>
                  <button 
                    className={`${styles.tabButton} ${activeTab === 'matchHistory' ? styles.tabButtonActive : ''}`}
                    onClick={() => setActiveTab('matchHistory')}
                  >
                    Match History
                  </button>
                  <button 
                    className={`${styles.tabButton} ${activeTab === 'rivals' ? styles.tabButtonActive : ''}`}
                    onClick={() => setActiveTab('rivals')}
                  >
                    Rivals
                  </button>
                  <button 
                    className={`${styles.tabButton} ${activeTab === 'Matchups' ? styles.tabButtonActive : ''}`}
                    onClick={() => setActiveTab('Matchups')}
                  >
                    Character Matchups
                  </button>
                </div>

                {activeTab === 'matchHistory' && (
                  <MatchHistory 
                    matches={matches} 
                    filters={filters} 
                    setFilters={setFilters} 
                    profile={profile}
                    opponentCharacters={opponentCharacters}
                  />
                )}

                {activeTab === 'rivals' && (
                  <Rivals 
                    rivals={rivals}
                    rivalsProfiles={rivalsProfiles}
                    filters={filters}
                    setFilters={setFilters}
                    profile={profile}
                    opponentCharacters={opponentCharacters}
                    tekkenId={tekkenId}
                  />
                )}

                {activeTab === 'Matchups' && (
                  <Matchups 
                    tekkenId={tekkenId}
                    profile={profile}
                  />
                )}
              </div>
            </div>

            <div className={styles.rightColumn}>
              <NameHistory names={profile.names} />
              <CharacterStatistics characters={profile.characters} />
            </div>
          </div>
        </div>
      </main>
    </div>
  );
};

export default TekkenStatsProfile;