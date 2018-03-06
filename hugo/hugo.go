package hugo

import (
	"io/ioutil"
	"log"
	"os"

	"github.com/gohugoio/hugo/deps"
	"github.com/gohugoio/hugo/helpers"
	"github.com/gohugoio/hugo/hugofs"
	"github.com/gohugoio/hugo/hugolib"
	jww "github.com/spf13/jwalterweatherman"
)

// Build Build a hugo project
func Build(projectDir string) error {
	var cfg = &deps.DepsCfg{}
	osFs := createFakeFs(hugofs.Os)
	//afero.NewOsFs
	//afero.NewCopyOnWriteFs()

	config, err := hugolib.LoadConfig(osFs, projectDir, "")
	if err != nil {
		return err
	}

	config.Set("workingDir", projectDir)

	cfg.Cfg = config

	logger := jww.NewNotepad(jww.LevelInfo, jww.LevelTrace, os.Stdout, ioutil.Discard, "", log.Ldate|log.Ltime)
	logger.SetLogThreshold(jww.LevelTrace)
	logger.SetStdoutThreshold(jww.LevelTrace)

	cfg.Logger = logger

	cfg.Fs = hugofs.NewFrom(osFs, config)

	//layer1 := afero.NewBasePathFs(osFs, "/home/pknopf/git/docgen/src/github.com/pauldotknopf/docgen/working")

	//layer2 := afero.NewBasePathFs(osFs, "/home/pknopf/git/test-pages")

	//combined := afero.NewCopyOnWriteFs(layer1, layer2)

	//cfg.Fs.WorkingDir = combined

	// s, err := cfg.Fs.WorkingDir.Stat("test")
	// if err != nil {
	// 	return err
	// }
	// fmt.Println(s.Name())

	//cfg.Fs.Destination = nil

	// baseFs := afero.NewBasePathFs(combined, "")
	// s, err = baseFs.Stat("test")
	// if err != nil {
	// 	return err
	// }

	// combined := afero.NewCopyOnWriteFs(cfg.Fs.WorkingDir, overlay)
	// s, err = overlay.Stat("test")
	// if err != nil {
	// 	return err
	// }

	// t := afero.NewBasePathFs(combined, "").(*afero.BasePathFs)

	// s, err = t.Stat("test")
	// if err != nil {
	// 	return err
	// }
	//fmt.Println(s.Name())

	config.Set("cacheDir", helpers.GetTempDir("hugo_cache", cfg.Fs.Source))

	sites, err := hugolib.NewHugoSites(*cfg)
	if err != nil {
		return err
	}

	//sites.Sites[0].

	return sites.Build(hugolib.BuildCfg{})
}
