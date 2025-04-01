const fs = require("node:fs");

fs.readFile("./characters.json", "utf8", (err, data) => {
  if (err) {
    console.error(err);
    return;
  }
  const json = JSON.parse(data);
  json.sort((a, b) => a.Id - b.Id);
  fs.writeFileSync("./characters.json", JSON.stringify(json));
});
