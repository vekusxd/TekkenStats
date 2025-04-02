import React from 'react';
import styles from '../TekkenStatsProfile.module.css';

const ProfileSummary = ({ profile }) => {
  return (
    <div className={styles.profileSummary}>
      <h1 className={styles.profileName}>{profile.currentName}</h1>
      <div className={styles.statsGrid}>
        <div className={styles.statItem}>
          <p className={styles.statValue}>{profile.matchesCount}</p>
          <p className={styles.statLabel}>Total Matches</p>
        </div>
        <div className={styles.statItem}>
          <p className={`${styles.statValue} ${styles.textGreen}`}>
            {Math.round((profile.winCount / profile.matchesCount) * 100)}%
          </p>
          <p className={styles.statLabel}>Win Rate</p>
        </div>
        <div className={styles.statItem}>
          <p className={styles.statValue}>
            {profile.characters.length > 0 ? profile.characters[0].characterName : 'N/A'}
          </p>
          <p className={styles.statLabel}>Main Character</p>
        </div>
      </div>
    </div>
  );
};

export default ProfileSummary;