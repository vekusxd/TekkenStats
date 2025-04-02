import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import axios from 'axios';
import styles from './TekkenStatsApp.module.css';
import Header from './Header';
import { BASE_URL } from '../config/baseUrl';

const Search = () => {
  const [searchQuery, setSearchQuery] = useState('');
  const [error, setError] = useState('');
  const navigate = useNavigate();

  const handleSearch = async (e) => {
    e.preventDefault();
    try {
      const response = await axios.get(`${BASE_URL}/api/names`, {
        params: {
          StartsWith: searchQuery,
          Amount: 1
        }
      });
      
      if (response.data.length > 0) {
        navigate(`/${response.data[0].tekkenId}`);
      } else {
        setError('Player not found');
      }
    } catch {
      setError('Error searching player');
    }
  };

  return (
    <div className={styles.appContainer}>
      <Header />
      
      <main className={styles.main}>
        <div className={styles.hero}>
          <h1 className={styles.title}>Find Your Fighter Stats</h1>
          <div className={styles.mainSearchContainer}>
            <form onSubmit={handleSearch} className={styles.searchForm}>
              <input
                type="text"
                placeholder="Search player name..."
                className={styles.searchInput}
                value={searchQuery}
                onChange={(e) => setSearchQuery(e.target.value)}
              />
              <button type="submit" className={styles.searchIcon}>
                <svg 
                  xmlns="http://www.w3.org/2000/svg" 
                  viewBox="0 0 24 24" 
                  fill="none" 
                  stroke="currentColor" 
                  strokeWidth="2" 
                  strokeLinecap="round" 
                  strokeLinejoin="round"
                >
                  <circle cx="11" cy="11" r="8"></circle>
                  <path d="m21 21-4.3-4.3"></path>
                </svg>
              </button>
            </form>
            {error && <p className={styles.errorMessage}>{error}</p>}
          </div>
        </div>
      </main>
    </div>
  );
};

export default Search;