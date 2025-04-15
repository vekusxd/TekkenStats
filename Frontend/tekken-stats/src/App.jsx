import { BrowserRouter, Route, Routes } from "react-router-dom";
import Search from "./components/Search";
import TekkenStatsProfile from "./components/TekkenStatsProfile";
import HeadToHead from "./components/HeadToHead";

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Search />} />
        <Route path="/player/:tekkenId" element={<TekkenStatsProfile />} />
        <Route path="/head-to-head/:tekkenId/:opponentTekkenId" element={<HeadToHead />} />
      </Routes>
    </BrowserRouter>
  )
}

export default App