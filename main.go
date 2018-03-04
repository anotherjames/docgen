package main

import (
	"fmt"
	"io/ioutil"
	"log"
	"os"

	"github.com/gohugoio/hugo/deps"
	"github.com/gohugoio/hugo/helpers"
	"github.com/gohugoio/hugo/hugofs"
	"github.com/gohugoio/hugo/hugolib"
	"github.com/gohugoio/hugo/source"
	jww "github.com/spf13/jwalterweatherman"
)

func main() {

	var cfg = &deps.DepsCfg{}
	osFs := hugofs.Os

	config, err := hugolib.LoadConfig(osFs, "/home/pknopf/git/test-pages", "")
	if err != nil {
		return
	}
	config.Set("workingDir", "/home/pknopf/git/test-pages")

	cfg.Cfg = config
	cfg.Logger = jww.NewNotepad(jww.LevelInfo, jww.LevelTrace, os.Stdout, ioutil.Discard, "", log.Ldate|log.Ltime)
	cfg.Logger.SetLogThreshold(jww.LevelTrace)
	cfg.Logger.SetStdoutThreshold(jww.LevelTrace)
	// if verboseLog {
	// 	logThreshold = jww.LevelInfo
	// 	if cfg.GetBool("debug") {
	// 		logThreshold = jww.LevelDebug
	// 	}
	// }

	// // The global logger is used in some few cases.
	// jww.SetLogOutput(logHandle)
	// jww.SetLogThreshold(logThreshold)
	// jww.SetStdoutThreshold(stdoutThreshold)

	fmt.Println("test")

	fs := hugofs.NewFrom(osFs, config)

	config.Set("cacheDir", helpers.GetTempDir("hugo_cache", fs.Source))

	cfg.Fs = fs

	ps, err := helpers.NewPathSpec(fs, cfg.Cfg)
	if err != nil {
		return
	}
	_ = ps

	staticDirsConfig, err := source.NewDirs(cfg.Fs, cfg.Cfg, cfg.Logger)
	if err != nil {
		return
	}
	_ = staticDirsConfig

	sites, err := hugolib.NewHugoSites(*cfg)
	if err != nil {
		return
	}
	_ = sites

	sites.Build(hugolib.BuildCfg{})
}
